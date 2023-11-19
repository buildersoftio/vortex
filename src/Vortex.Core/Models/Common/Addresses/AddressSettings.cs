namespace Vortex.Core.Models.Common.Addresses
{
    public class AddressSettings
    {
        // 
        public AddressScope Scope { get; set; }

        public bool EnforceSchemaValidation { get; set; }

        // we will use this property to store in LiteDB based on the type the starting Id of that time.
        // if the MessageIndexType will be daily, for each day we will log the starting Id of that day.
        // Index can not change after addess is created
        public MessageIndexTypes MessageIndexType { get; set; }

        // Which node is leader for partition
        public AddressStorageSettings StorageSettings { get; set; }

        public AddressPartitionSettings PartitionSettings { get; set; }
        public AddressReplicationSettings ReplicationSettings { get; set; }
        public AddressRetentionSettings RetentionSettings { get; set; }
        public AddressSchemaSettings SchemaSettings { get; set; }

        public AddressSettings()
        {
            // the default values should come from AddressService
            Scope = AddressScope.SingleScope;

            EnforceSchemaValidation = false;
            MessageIndexType = MessageIndexTypes.DAILY;

            SchemaSettings = new AddressSchemaSettings();
            PartitionSettings = new AddressPartitionSettings();
            ReplicationSettings = new AddressReplicationSettings();
            RetentionSettings = new AddressRetentionSettings();
            SchemaSettings = new AddressSchemaSettings();
        }
    }
}
