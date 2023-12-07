using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.Factories;
using Vortex.Core.IO.Services;
using Vortex.Core.Models.BackgroundRequests;
using Vortex.Core.Models.Common.Clusters;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Utilities.Consts;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace Vortex.Core.Services.Clustering
{
    public class ClusterManager : IClusterManager
    {
        private readonly ILogger<ClusterManager> _logger;
        private readonly IClusterStateRepository _clusterStateRepository;
        private readonly IConfigIOService _configIOService;
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly INodeExchangeClientFactory _nodeExchangeClientFactory;
        private readonly ITimedBackgroundService<HeartbeatTimerRequest> _timedBackgroundService;

        public ClusterManager(ILogger<ClusterManager> logger,
            IClusterStateRepository clusterStateRepository,
            IConfigIOService configIOService,
            NodeConfiguration nodeConfiguration,
            INodeExchangeClientFactory nodeExchangeClientFactory,
            ITimedBackgroundService<HeartbeatTimerRequest> timedBackgroundService)
        {
            _logger = logger;
            _clusterStateRepository = clusterStateRepository;
            _configIOService = configIOService;
            _nodeConfiguration = nodeConfiguration;
            _nodeExchangeClientFactory = nodeExchangeClientFactory;
            _timedBackgroundService = timedBackgroundService;
        }

        public void Start()
        {
            _logger.LogInformation("Configuring Cluster...");


            var clusterConfiguration = _configIOService.GetClusterConfiguration();
            _logger.LogInformation("Changing state of default cluster configuration");

            _clusterStateRepository.UpdateClusterName(clusterConfiguration!.Name);
            _clusterStateRepository.UpdateClusterStatus(ClusterStatus.Offline);

            if (clusterConfiguration!.Nodes.Count == 1)
            {
                //TODO: vortex_standalone should be somehow integrated with node_id; in cse there is just one node.
                
                // we are removing this condition for now, in case we have just a single node registered in the 

                //if (clusterConfiguration.Nodes.ContainsKey("vortex_standalone"))
                //{
                _clusterStateRepository.UpdateClusterStatus(ClusterStatus.OnlineStandalone);
                _logger.LogInformation($"Cluster identifier is 'Standalone', only one node is running within this cluster");

                var nodeToCreate = new Node()
                {
                    Id = _nodeConfiguration.NodeId,
                    Address = "localhost",
                    Port = 0,
                    //State = NodeState.Follower,
                    LastHeartbeat = DateTime.Now,
                    Status = NodeStatus.Online
                };

                _clusterStateRepository.AddNode(_nodeConfiguration.NodeId, nodeToCreate);

                //}
            }
            else
            {
                // Node is part of more than one Cluster
                _clusterStateRepository.UpdateClusterStatus(ClusterStatus.Initializing);

                _logger.LogInformation($"Cluster configuration contains multiple node configurations");
                int k = 0;

                foreach (var node in clusterConfiguration.Nodes)
                {
                    if (node.Key == _nodeConfiguration.NodeId)
                        continue;

                    var nodeToCreate = new Node()
                    {
                        Id = node.Key,
                        Address = node.Value.Address,
                        Port = node.Value.Port,
                        //State = NodeState.Follower,
                        LastHeartbeat = DateTime.Now,
                        Status = NodeStatus.Offline
                    };

                    _clusterStateRepository.AddNode(node.Key, nodeToCreate);
                    _clusterStateRepository.AddNodeClient(node.Key, _nodeExchangeClientFactory.CreateNodeExchangeClient(nodeToCreate, _nodeConfiguration));


                    _logger.LogInformation($"Cluster identifier is '{clusterConfiguration.Name}' and contains [{node.Value.Address}] integration");
                    k++;
                }

                _logger.LogInformation($"Loading {SystemProperties.Name} in cluster mode ...");
                _logger.LogInformation($"Loading...");
                _logger.LogInformation($"Loading...");
                _logger.LogInformation($"Loading...");
                _logger.LogInformation($"Loading...");
                _logger.LogInformation($"Starting Cluster...");

                if (_clusterStateRepository.GetNodes().Count > 0)
                {
                    // more than one node is connected already

                    foreach (var neighborNode in _clusterStateRepository.GetNodes())
                    {
                        _logger.LogInformation($"Requesting node '{neighborNode.Key}' exchange connection");
                        var neighborNodeClient = _clusterStateRepository.GetNodeClient(neighborNode.Key);
                        var isConnected = neighborNodeClient!.ConnectAsync().Result;
                        if (isConnected == true)
                        {
                            _clusterStateRepository.UpdateClusterStatus(ClusterStatus.Online);
                        }
                    }

                    _logger.LogInformation($"Starting Heartbeat background services...");

                    _timedBackgroundService.Start();
                    _logger.LogInformation($"Heartbeat background services started");
                }
            }
        }
    }
}
