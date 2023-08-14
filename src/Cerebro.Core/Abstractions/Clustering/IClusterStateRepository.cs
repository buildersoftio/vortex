using Cerebro.Core.Models.Common.Clusters;
using System.Collections.Concurrent;

namespace Cerebro.Core.Abstractions.Clustering
{
    public interface IClusterStateRepository
    {
        void UpdateClusterName(string clusterName);
        void UpdateClusterStatus(ClusterStatus clusterStatus);

        void AddNode(string nodeId, Node node);
        void EditNode(string nodeId, Node node);
        void UpdateHeartBeat(string nodeId);
        void UpdateNodeStatus(string nodeId, NodeStatus nodeStatus);
        void UpdateNodeState(string nodeId, NodeState nodeState);

        ConcurrentDictionary<string, Node> GetNodes();
        ConcurrentDictionary<string, INodeExchangeClient> GetNodeClients();

        void AddNodeClient(string nodeId, INodeExchangeClient node);
        void RemoveNodeClient(string nodeId);
        INodeExchangeClient? GetNodeClient(string nodeId);

        DateTime? GetLastHeartbeat(string nodeId);
        Node? GetNode(string nodeId);
    }
}
