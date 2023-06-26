namespace Cerebro.Core.Models.Clustering
{
    public class Node
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }

        // Additional Properties
        public string Name { get; set; }
        public NodeStatus Status { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}
