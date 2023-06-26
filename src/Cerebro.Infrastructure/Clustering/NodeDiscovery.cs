using Cerebro.Core.Clustering;
using Cerebro.Core.Models.Clustering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cerebro.Infrastructure.Clustering
{
    public class NodeDiscovery : INodeDiscovery
    {
        private readonly ILogger<NodeDiscovery> _logger;
        private readonly UdpClient _udpClient;
        private readonly IClusterManager _clusterManager;
        private readonly int _broadcastPort;
        private readonly string _broadcastAddress;
        private readonly TimeSpan _heartbeatInterval;
        private readonly TimeSpan _offlineThreshold;
        private readonly Timer _heartbeatTimer;
        private readonly Dictionary<string, DateTime> _lastHeartbeats = new Dictionary<string, DateTime>();
        private readonly object _lockObject = new object();


        public NodeDiscovery(ILogger<NodeDiscovery> logger, IClusterManager clusterManager, int broadcastPort, string broadcastAddress, TimeSpan heartbeatInterval, TimeSpan offlineThreshold)
        {
            _logger = logger;
            _udpClient = new UdpClient();
            _clusterManager = clusterManager;
            _broadcastPort = broadcastPort;
            _broadcastAddress = broadcastAddress;
            _heartbeatInterval = heartbeatInterval;
            _offlineThreshold = offlineThreshold;
            _heartbeatTimer = new Timer(HeartbeatTimerCallback, null, TimeSpan.Zero, heartbeatInterval);
        }

        public void StartListening()
        {
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, _broadcastPort));

            StartReceiving();
        }

        public void SendNodeBroadcast(Node node)
        {
            var nodeData = JsonConvert.SerializeObject(node);
            var messageBytes = Encoding.UTF8.GetBytes(nodeData);

            _udpClient.Send(messageBytes, messageBytes.Length, _broadcastAddress, _broadcastPort);
        }

        private void StartReceiving()
        {
            _udpClient.BeginReceive(ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var remoteEndPoint = new IPEndPoint(IPAddress.Any, _broadcastPort);
            var receivedBytes = _udpClient.EndReceive(ar, ref remoteEndPoint);

            var receivedData = Encoding.UTF8.GetString(receivedBytes);
            var node = JsonConvert.DeserializeObject<Node>(receivedData);

            lock (_lockObject)
            {
                if (!_lastHeartbeats.ContainsKey(node.Id))
                {
                    _lastHeartbeats.Add(node.Id, DateTime.UtcNow);
                    _clusterManager.RegisterNode(node);
                    Console.WriteLine($"Received node broadcast: {node.Id}");
                }
                else
                {
                    _lastHeartbeats[node.Id] = DateTime.UtcNow;
                    _clusterManager.UpdateHeartbeat(node.Id);
                    Console.WriteLine($"Updated heartbeat for node: {node.Id}");
                }
            }

            StartReceiving();
        }

        private void HeartbeatTimerCallback(object state)
        {
            var currentNodes = _clusterManager.GetAvailableNodes();

            lock (_lockObject)
            {
                var currentTime = DateTime.UtcNow;

                foreach (var node in currentNodes)
                {
                    if (!_lastHeartbeats.ContainsKey(node.Id))
                    {
                        _lastHeartbeats.Add(node.Id, currentTime);
                    }
                    else
                    {
                        var lastHeartbeat = _lastHeartbeats[node.Id];
                        var heartbeatDiff = currentTime - lastHeartbeat;

                        if (heartbeatDiff > _offlineThreshold)
                        {
                            Console.WriteLine($"Node offline: {node.Id}");

                            // Perform actions for an offline node, such as removing it from the cluster or marking it as offline
                            HandleOfflineNode(node);


                            _lastHeartbeats.Remove(node.Id);
                            _clusterManager.RemoveNode(node.Id);
                        }
                    }
                }
            }
        }

        private void HandleOfflineNode(Node node)
        {
            // Example actions for an offline node
            // 1. Notify other nodes about the offline node
            NotifyNodesAboutOfflineNode(node);

            // 2. Update database or data store to reflect the offline node status
            UpdateDataStoreForOfflineNode(node);

            // 3. Trigger any necessary cleanup or reassignment processes
            PerformCleanupForOfflineNode(node);

            // You can add more actions as per your application's requirements
        }

        private void NotifyNodesAboutOfflineNode(Node offlineNode)
        {
            var availableNodes = _clusterManager.GetAvailableNodes();

            foreach (var node in availableNodes)
            {
                // Send a message or notification to each available node about the offline node
                // You can use gRPC, SignalR, or any other communication mechanism
                // to notify other nodes about the offline node
                SendMessageToNode(node, $"Node {offlineNode.Id} is offline");
            }
        }

        private void UpdateDataStoreForOfflineNode(Node offlineNode)
        {
            // Update your database or data store to reflect the offline node status
            // For example, mark the node as offline or update its status in the database
            // You can use your preferred data access mechanism (e.g., Entity Framework, ADO.NET) to perform the update
        }

        private void PerformCleanupForOfflineNode(Node offlineNode)
        {
            // Perform any necessary cleanup or reassignment processes
            // This could include redistributing tasks or reassigning responsibilities
            // among the remaining nodes in the cluster
            // You can implement your custom logic here based on your application's requirements
        }

        private void SendMessageToNode(Node node, string message)
        {
            // Use your preferred communication mechanism (e.g., gRPC, SignalR) to send a message to the specified node
            // You can implement the appropriate logic based on the communication mechanism you are using
        }

        private void HandleNodeReconnection(Node node)
        {
            // Perform actions for a node that is back online
            // This could include updating the data store, redistributing tasks,
            // or resuming normal operations that were suspended when the node went offline
            // Implement your custom logic here based on your application's requirements
        }
    }
}
