namespace Vortex.Core.Models.Common.Addresses
{
    public enum AddressStatuses
    {
        // ADD states for each step, create_directories, create_rocks_db_for_partitions, create_replication_state_storage, create_indexes_state
        CreateAddressDirectory,
        CreatePartitionDirectories,
        Ready,
        Idle,
        NoStorageLeft,
        Blocked,
        ChangePartitions,
        DeletePartitions,
        Failed
    }
}
