using Cerebro.Core.Models.Entities.Clustering;

namespace Cerebro.Core.Clustering
{
    public interface INodeDiscovery
    {
        void StartListening();
        void SendNodeBroadcast(Node node);

    }
}
