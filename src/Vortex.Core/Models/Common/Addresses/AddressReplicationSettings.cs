namespace Vortex.Core.Models.Common.Addresses
{
    public class AddressReplicationSettings
    {

        // How many replicas the users wants to have in the cluster.
        // max number of replicas is max number of nodes in the cluster.
        public int ReplicationFactor { get; set; }
    }
}
