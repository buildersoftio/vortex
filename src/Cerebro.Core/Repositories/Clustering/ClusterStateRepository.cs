using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.Models.Common.Clusters;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Cerebro.Core.Repositories.Clustering
{
    public class ClusterStateRepository : IClusterStateRepository
    {
        private readonly ILogger<ClusterStateRepository> _logger;
        private readonly Cluster _currentClusterState;

        public ClusterStateRepository(ILogger<ClusterStateRepository> logger)
        {
            _logger = logger;

            _currentClusterState = new Cluster();
        }
        public void AddNode(string nodeId, Node node)
        {
            if (_currentClusterState.Nodes.ContainsKey(nodeId) != true)
            {
                _currentClusterState.Nodes.TryAdd(nodeId, node);
            }
        }

        public void AddNodeClient(string nodeId, INodeExchangeClient node)
        {
            if (_currentClusterState.NodeExchangeClients.ContainsKey(nodeId) != true)
                _currentClusterState.NodeExchangeClients.TryAdd(nodeId, node);
        }

        public void EditNode(string nodeId, Node node)
        {
            if (_currentClusterState.Nodes.ContainsKey(nodeId) != true)
                return;

            _currentClusterState.Nodes[nodeId] = node;
        }

        public DateTime? GetLastHeartbeat(string nodeId)
        {
            if (_currentClusterState.Nodes.ContainsKey(nodeId) != true)
                return null;

            return _currentClusterState.Nodes[nodeId].LastHeartbeat;
        }

        public Node? GetNode(string nodeId)
        {
            if (_currentClusterState.Nodes.ContainsKey(nodeId) != true)
                return null;

            return _currentClusterState.Nodes[nodeId];
        }

        public INodeExchangeClient? GetNodeClient(string nodeId)
        {
            if (_currentClusterState.NodeExchangeClients.ContainsKey(nodeId) != true)
                return null;

            return _currentClusterState.NodeExchangeClients[nodeId];
        }

        public ConcurrentDictionary<string, INodeExchangeClient> GetNodeClients()
        {
            return _currentClusterState.NodeExchangeClients;
        }

        public ConcurrentDictionary<string, Node> GetNodes()
        {
            return _currentClusterState.Nodes;
        }

        public void RemoveNodeClient(string nodeId)
        {
            if (_currentClusterState.NodeExchangeClients.ContainsKey(nodeId) != true)
                return;

            _currentClusterState.NodeExchangeClients.TryRemove(nodeId, out _);
        }

        public void UpdateClusterName(string clusterName)
        {
            _currentClusterState.Name = clusterName;
        }

        public void UpdateClusterStatus(ClusterStatus clusterStatus)
        {
            _currentClusterState.Status = clusterStatus;
        }

        public void UpdateHeartBeat(string nodeId)
        {
            if (_currentClusterState.Nodes.ContainsKey(nodeId) != true)
                return;

            _currentClusterState.Nodes[nodeId].LastHeartbeat = DateTime.Now;
        }

        //public void UpdateNodeState(string nodeId, NodeState nodeState)
        //{
        //    if (_currentClusterState.Nodes.ContainsKey(nodeId) != true)
        //        return;

        //    _currentClusterState.Nodes[nodeId].State = nodeState;
        //}

        public void UpdateNodeStatus(string nodeId, NodeStatus nodeStatus)
        {
            if (_currentClusterState.Nodes.ContainsKey(nodeId) != true)
                return;

            _currentClusterState.Nodes[nodeId].Status = nodeStatus;
        }
    }
}
