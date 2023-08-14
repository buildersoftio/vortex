using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.Models.Configurations;
using Cerebro.Core.Utilities.Consts;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NodeExchange;

namespace Cerebro.Cluster.Infrastructure.Servers
{
    public class gRPCNodeExchangeServer : NodeExchange.NodeExchangeService.NodeExchangeServiceBase, INodeExchangeServer
    {
        private readonly ILogger<gRPCNodeExchangeServer> _logger;

        private readonly int _port;
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly Server _server;
        private readonly IClusterStateRepository _clusterStateRepository;

        public gRPCNodeExchangeServer(ILogger<gRPCNodeExchangeServer> logger, NodeConfiguration nodeConfiguration, IClusterStateRepository clusterStateRepository)
        {
            _logger = logger;
            _nodeConfiguration = nodeConfiguration;
            _clusterStateRepository = clusterStateRepository;


            if (Environment.GetEnvironmentVariable(EnvironmentConstants.CerebroClusterConnectionPort) != null)
                _port = Convert.ToInt32(Environment.GetEnvironmentVariable(EnvironmentConstants.CerebroClusterConnectionPort));
            else
                _port = -1;

            string? hostName = Environment.GetEnvironmentVariable(EnvironmentConstants.CerebroClusterHost);
            hostName ??= "localhost";

            _server = new Server()
            {
                Services = { NodeExchange.NodeExchangeService.BindService(this) },
                Ports = { new ServerPort(hostName, _port, ServerCredentials.Insecure) }
            };
        }

        public Task ShutdownAsync()
        {
            if (_port != -1)
            {
                _logger.LogInformation($"Cluster node '{_nodeConfiguration.NodeId}' is shutdown");
                return _server.ShutdownAsync();
            }

            return Task.CompletedTask;
        }

        public void Start()
        {
            if (_port != -1)
            {
                _server.Start();
                _logger.LogInformation($"Cluster listening on port " + _port + " for node " + _nodeConfiguration.NodeId);
            }
        }

        public override Task<NodeRegistrationResponse> RegisterNode(NodeInfo request, ServerCallContext context)
        {
            _logger.LogInformation($"Node {request.NodeId} connection requested");
            return Task.FromResult(new NodeRegistrationResponse() { Message = "Node connected", Success = true });
        }

        public override Task<HeartbeatResponse> SendHeartbeat(Heartbeat request, ServerCallContext context)
        {
            _logger.LogInformation($"Heartbeat requested by node {request.NodeId}");
            return Task.FromResult(new HeartbeatResponse() { Success = true, Message = "Heartbeat received" });
        }
    }
}
