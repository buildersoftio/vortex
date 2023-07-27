namespace Cerebro.Core.Models.Common.Addresses
{
    public class AddressReplicationSettings
    {
        public string NodeIdLeader { get; set; }

        // in case of * it means it should replicate data to all nodes
        // e.g.,  [partitionId]:[brokerId],[partitionId]:[brokerId],…
        public string FollowerReplicationReplicas { get; set; }
    }
}
