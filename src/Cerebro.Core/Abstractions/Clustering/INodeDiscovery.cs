using Cerebro.Core.Models.Clustering;

namespace Cerebro.Core.Clustering
{
    public interface INodeDiscovery
    {
        void StartListening();
        void SendNodeBroadcast(Node node);

    }
}
