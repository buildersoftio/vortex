using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Models.Common.Clusters;
using Vortex.Core.Models.Configurations;

namespace Vortex.Core.Abstractions.Factories
{
    public interface INodeExchangeClientFactory
    {
        INodeExchangeClient CreateNodeExchangeClient(Node node, NodeConfiguration nodeConfiguration);
    }
}
