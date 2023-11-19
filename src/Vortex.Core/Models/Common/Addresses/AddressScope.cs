namespace Vortex.Core.Models.Common.Addresses
{
    public enum AddressScope
    {
        SingleScope,    // Address is for a single node
        ClusterScope    // Address is created across the cluster, which determinates the replication factor and node <-> parition priority
    }
}
