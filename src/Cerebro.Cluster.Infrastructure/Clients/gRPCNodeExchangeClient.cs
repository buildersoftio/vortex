using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.Models.Common.Clusters;
using Cerebro.Core.Models.Configurations;
using Grpc.Core;
using System.Xml.Linq;

namespace Cerebro.Cluster.Infrastructure.Clients
{
    public class gRPCNodeExchangeClient : INodeExchangeClient
    {
        private readonly Node _node;
        private readonly NodeConfiguration _serverNodeConfiguration;
        private readonly Channel _channel;
        private readonly NodeExchange.NodeExchangeService.NodeExchangeServiceClient _client;

        public gRPCNodeExchangeClient(Node node, NodeConfiguration nodeConfiguration)
        {
            _node = node;
            _serverNodeConfiguration = nodeConfiguration;

            _channel = new Channel($"{node.Address}:{node.Port}", ChannelCredentials.Insecure);
            _client = new NodeExchange.NodeExchangeService.NodeExchangeServiceClient(_channel);
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                var response = await _client.RegisterNodeAsync(new NodeExchange.NodeInfo() { NodeId = _serverNodeConfiguration.NodeId });
                if (response.Success != true)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to node {_node.Id}, error details: {ex.Message}");
            }


            return false;
        }

        public Task DisconnectAsync()
        {
            //TODO: figure it out how to disconnect the client
            return Task.CompletedTask;
        }

        public async Task<bool> RequestHeartBeatAsync()
        {
            var result = await _client.SendHeartbeatAsync(new NodeExchange.Heartbeat() { NodeId = _serverNodeConfiguration.NodeId });
            return true;
        }
    }
}
