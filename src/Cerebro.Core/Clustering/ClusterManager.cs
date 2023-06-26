using Cerebro.Core.Models.Clustering;
using Microsoft.Extensions.Logging;

namespace Cerebro.Core.Clustering
{
    public class ClusterManager : IClusterManager
    {
        private readonly ILogger<ClusterManager> _logger;

        private readonly Dictionary<string, Node> _Nodes;
        private readonly object _lockObject = new object();

        public ClusterManager(ILogger<ClusterManager> logger)
        {
            _Nodes = new Dictionary<string, Node>();
            _logger = logger;
        }

        public void RegisterNode(Node node)
        {
            lock (_lockObject)
            {
                if (!_Nodes.ContainsKey(node.Id))
                {
                    _Nodes.Add(node.Id, node);
                    Console.WriteLine($"Node registered: {node.Id}");
                }
            }
        }

        public void RemoveNode(string nodeId)
        {
            lock (_lockObject)
            {
                if (_Nodes.ContainsKey(nodeId))
                {
                    _Nodes.Remove(nodeId);
                    Console.WriteLine($"Node removed: {nodeId}");
                }
            }
        }

        public void UpdateHeartbeat(string nodeId)
        {
            lock (_lockObject)
            {
                if (_Nodes.ContainsKey(nodeId))
                {
                    _Nodes[nodeId].LastHeartbeat = DateTime.UtcNow;
                }
            }
        }
        public List<Node> GetAvailableNodes()
        {
            lock (_lockObject)
            {
                return _Nodes.Values.ToList();
            }
        }

        public bool IsNodeRegistered(string nodeId)
        {
            lock (_lockObject)
            {
                return _Nodes.ContainsKey(nodeId);
            }
        }

        public List<string> GetOfflineNodes(TimeSpan offlineThreshold)
        {
            lock (_lockObject)
            {
                var currentTime = DateTime.UtcNow;
                return _Nodes
                    .Where(kv => (currentTime - kv.Value.LastHeartbeat) > offlineThreshold)
                    .Select(kv => kv.Key)
                    .ToList();
            }
        }
    }
}
