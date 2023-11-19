using Cerebro.Core.Models.Common.Addresses;
using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Dtos.Addresses;
using Cerebro.Core.Models.Dtos.Applications;
using Cerebro.Core.Models.Entities.Clients.Applications;

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

        Task<bool> RequestApplicationCreation(ApplicationDto applicationDto, string createdBy);
        Task<bool> RequestApplicationSoftDeletion(string applicationName, string updatedBy);
        Task<bool> RequestApplicationHardDeletion(string applicationName);
        Task<bool> RequestApplicationDescriptionChange(string applicationName, string description, string updatedBy);
        Task<bool> RequestApplicationSettingsChange(string applicationName, ApplicationSettings applicationSettings, string updatedBy);

        Task<bool> RequestApplicationStatusChange(string applicationName, bool status, string updatedBy);
        Task<bool> RequestApplicationTokenCreation(string applicationName, ApplicationToken applicationToken, string createdBy);
        Task<bool> RequestApplicationTokenRevocation(string applicationName, Guid apiKey, string updatedBy);
        Task<bool> RequestApplicationPermissionChange(string applicationName, string permissionType, string value, string updatedBy);
    }
}