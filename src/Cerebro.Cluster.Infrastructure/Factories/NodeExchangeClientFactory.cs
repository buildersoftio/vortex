using Cerebro.Cluster.Infrastructure.Clients;
using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.Abstractions.Factories;
using Cerebro.Core.Models.Common.Clusters;
using Cerebro.Core.Models.Configurations;

namespace Cerebro.Cluster.Infrastructure.Factories
{
    public class NodeExchangeClientFactory : INodeExchangeClientFactory
    {
        public INodeExchangeClient CreateNodeExchangeClient(Node node, NodeConfiguration nodeConfiguration)
        {
            return new gRPCNodeExchangeClient(node, nodeConfiguration);
        }
    }
}
