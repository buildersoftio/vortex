using Cerebro.Core.Abstractions.Background;
using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.IO.Services;
using Cerebro.Core.Models.BackgroundRequests;
using Microsoft.Extensions.Logging;

namespace Cerebro.Core.Services.Clustering.Background
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
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to requeset Heartbeat to node {node.Key}, error details: {ex.Message}");
                }
            }
        }
    }
}
