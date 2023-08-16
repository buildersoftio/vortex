using Cerebro.Core.Models.Common.Addresses;
using Cerebro.Core.Models.Dtos.Addresses;

namespace Cerebro.Core.Abstractions.Clustering
{
    public interface INodeExchangeClient
    {
        Task<bool> ConnectAsync();
        Task DisconnectAsync();

        Task<bool> RequestHeartBeatAsync();

        Task<bool> RequestAddressCreation(AddressClusterScopeRequest request);
        Task<bool> RequestAddressPartitionChange(string alias, int partitionNumner, string updatedBy);
        Task<bool> RequestAddressRetentionSettingsChange(string alias, AddressRetentionSettings addressRetentionSettings, string updatedBy);
        Task<bool> RequestAddressSchemaSettingsChange(string alias, AddressSchemaSettings addressSchemaSettings, string updatedBy);
        Task<bool> RequestAddressStorageSettingsChange(string alias, AddressStorageSettings addressStorageSettings, string updatedBy);
        Task<bool> RequestAddressReplicationSettingsChange(string alias, AddressReplicationSettings addressReplicationSettings, string updatedBy);
        Task<bool> RequestAddressDeletion(string alias);
    }
}