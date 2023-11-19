using Vortex.Cluster.Infrastructure.Clients;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.Factories;
using Vortex.Core.Models.Common.Clusters;
using Vortex.Core.Models.Configurations;

namespace Vortex.Cluster.Infrastructure.Factories
{
    public class NodeExchangeClientFactory : INodeExchangeClientFactory
    {
        public INodeExchangeClient CreateNodeExchangeClient(Node node, NodeConfiguration nodeConfiguration)
        {
            return new gRPCNodeExchangeClient(node, nodeConfiguration);
        }
    }
}
