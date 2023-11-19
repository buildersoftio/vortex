using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.Models.Common.Clusters;
using Cerebro.Core.Models.Configurations;

namespace Cerebro.Core.Abstractions.Factories
{
    public interface INodeExchangeClientFactory
    {
        INodeExchangeClient CreateNodeExchangeClient(Node node, NodeConfiguration nodeConfiguration);
    }
}
