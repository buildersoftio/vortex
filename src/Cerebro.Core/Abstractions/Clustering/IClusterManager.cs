using Cerebro.Core.Models.Clustering;

namespace Cerebro.Core.Clustering
{
    public interface IClusterManager
    {
        void RegisterNode(Node node);
        void RemoveNode(string nodeId);
        void UpdateHeartbeat(string nodeId);
        List<Node> GetAvailableNodes();
        bool IsNodeRegistered(string nodeId);
        List<string> GetOfflineNodes(TimeSpan offlineThreshold);
    }
}
