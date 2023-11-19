namespace Cerebro.Core.Models.Common.Clusters
{
    public class ClusterSettings
    {
        public int HeartbeatInterval { get; set; }
        public int HeartbeatTimeout { get; set; }

        public PartitionDistributionTypes PartitionDistribution { get; set; }

        public ClusterSettings()
        {
            HeartbeatInterval = 10;
            HeartbeatTimeout = 30;
            PartitionDistribution = PartitionDistributionTypes.RoundRobin;
        }
    }

    public enum PartitionDistributionTypes
    {
        RoundRobin,

        // v4.0.0 only round-robin pratition distribution is supported.
        // RackDriven
    }
}
