namespace Cerebro.Core.Models.Entities.Addresses
{
    public class AddressSettings
    {
        public bool EnforceSchemaValidation { get; set; }

        // we will use this property to store in LiteDB based on the type the starting Id of that time.
        // if the MessageIndexType will be daily, for each day we will log the starting Id of that day.
        public MessageIndexTypes MessageIndexType { get; set; }

        // Which node is leader for partition
        public AddressStorageSettings StorageSettings { get; set; }

        public AddressPartitionSettings PartitionSettings { get; set; }
        public AddressReplicationSettings ReplicationSettings { get; set; }
        public AddressRetentionSettings RetentionSettings { get; set; }
        public AddressSchemaSettings SchemaSettings { get; set; }

        public AddressSettings()
        {
            SchemaSettings = new AddressSchemaSettings();
        }
    }

    public class AddressSchemaSettings
    {
        public int SchemaId { get; set; }
        public CompatibilityTypes CompatibilityType { get; set; }

        public AddressSchemaSettings()
        {
            // move this to settings as default parameter when address is created
            CompatibilityType = CompatibilityTypes.NONE;
        }
    }

    public enum CompatibilityTypes
    {
        NONE,
        BACKWARD,
        BACKWARD_TRANSITIVE,
        FORWARD,
        FORWARD_TRANSITIVE,
        FULL,
        FULL_TRANSITIVE,
    }

    public class AddressStorageSettings
    {
        public ulong WriteBufferSizeInBytes { get; set; }
        public int MaxWriteBufferNumber { get; set; }
        public int MaxWriteBufferSizeToMaintain { get; set; }
        public int MinWriteBufferNumberToMerge { get; set; }
        public int MaxBackgroundCompactionsThreads { get; set; }
        public int MaxBackgroundFlushesThreads { get; set; }
    }

    public class AddressPartitionSettings
    {
        public int PartitionNumber { get; set; }

    }

    public class AddressReplicationSettings
    {
        public string NodeIdLeader { get; set; }

        // in case of * it means it should replicate data to all nodes
        // e.g.,  [partitionId]:[brokerId],[partitionId]:[brokerId],…
        public string FollowerReplicationReplicas { get; set; }
    }

    public class AddressRetentionSettings
    {
        public RetentionTypes RetentionType { get; set; }
        public long TimeToLiveInMinutes { get; set; }

        public AddressRetentionSettings()
        {
            RetentionType = RetentionTypes.DELETE;
            TimeToLiveInMinutes = -1;
        }
    }

    public enum RetentionTypes
    {
        DELETE,
        ARCHIVE
    }

    public enum MessageIndexTypes
    {
        HOURLY,
        DAILY,
        MONTHLY
    }

    public enum CompressionType
    {
        NONE,
        ZSTD,
        LZ4
    }
}
