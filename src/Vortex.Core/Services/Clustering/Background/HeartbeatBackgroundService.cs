﻿using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.IO.Services;
using Vortex.Core.Models.BackgroundRequests;
using Microsoft.Extensions.Logging;
using Vortex.Core.Models.Common.Clusters;

namespace Vortex.Core.Services.Clustering.Background
{
    public class HeartbeatBackgroundService : TimedBackgroundServiceBase<HeartbeatTimerRequest>
    {
        private readonly ILogger<HeartbeatBackgroundService> _logger;
        private readonly IClusterStateRepository _clusterStateRepository;
        private readonly IConfigIOService _configIOService;

        private static TimeSpan GetPeriod(IConfigIOService configIOService)
        {
            return new TimeSpan(0, 0, configIOService.GetClusterConfiguration()!.Settings.HeartbeatInterval);
        }

        public HeartbeatBackgroundService(ILogger<HeartbeatBackgroundService> logger, IClusterStateRepository clusterStateRepository, IConfigIOService configIOService) : base(GetPeriod(configIOService))
        {
            _logger = logger;
            _clusterStateRepository = clusterStateRepository;
            _configIOService = configIOService;
        }

        public override async void OnTimer_Callback(object? state)
        {
            foreach (var node in _clusterStateRepository.GetNodes())
            {
                try
                {
                    var client = _clusterStateRepository.GetNodeClient(node.Key);
                    await client!.RequestHeartBeatAsync();

                    _clusterStateRepository.UpdateHeartBeat(node.Key);
                    _clusterStateRepository.UpdateNodeStatus(node.Key, NodeStatus.Online);
                    _clusterStateRepository.UpdateClusterStatus(ClusterStatus.Online);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to request Heartbeat to node {node.Key}, error details: {ex.Message}");
                    _clusterStateRepository.UpdateClusterStatus(ClusterStatus.Reconnecting);
                    _clusterStateRepository.UpdateNodeStatus(node.Key, NodeStatus.Reconnecting);

                    // checking timeout
                    var lastHeartBeat = node.Value.LastHeartbeat!.Value.TimeOfDay;
                    var rightNow = DateTime.Now.TimeOfDay;
                    var difference = rightNow - lastHeartBeat;
                    if (difference.TotalSeconds >= _configIOService.GetClusterConfiguration()!.Settings.HeartbeatTimeout)
                    {
                        _logger.LogError($"Node {node.Key} is offline, trying to connect in {_configIOService.GetClusterConfiguration()!.Settings.HeartbeatInterval} seconds");
                        _clusterStateRepository.UpdateClusterStatus(ClusterStatus.Offline);
                        _clusterStateRepository.UpdateNodeStatus(node.Key, NodeStatus.Offline);
                    }
                }
            }
        }

        private void CheckHeartBeatTimeout()
        {

        }
    }
}
