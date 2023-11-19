using Cerebro.Core.Models.Common.Addresses;
using Cerebro.Core.Models.Dtos.Addresses;

namespace Cerebro.Core.Abstractions.Services
{
    public interface IAddressService
    {
        (bool status, string message) CreateDefaultAddress(AddressDefaultCreationRequest addressDefaultCreationRequest, string createdBy, bool requestedByOtherNode = false);
        (bool status, string message) CreateAddress(AddressCreationRequest addressCreationRequest, string createdBy, bool requestedByOtherNode = false);
        (bool status, string message) DeleteAddress(string alias, bool requestedByOtherNode = false);
        (bool status, string message) PromoteAddress(string alias, string updatedBy, bool requestedByOtherNode = false);

        (AddressDto? address, string message) GetAddressByAlias(string alias);
        (AddressDto? address, string message) GetAddressByName(string alias);
        (List<AddressDto>? addresses, string message) GetAddresses();

        (bool status, string message) EditAddressStorageSettings(string alias, AddressStorageSettings addressStorageSettings, string updatedBy, bool requestedByOtherNode = false);
        (bool status, string message) EditAddressPartitionSettings(string alias, AddressPartitionSettings addressPartitionSettings, string updatedBy, bool requestedByOtherNode = false);
        (bool status, string message) EditAddressReplicationSettings(string alias, AddressReplicationSettings addressReplicationSettings, string updatedBy, bool requestedByOtherNode = false);
        (bool status, string message) EditAddressRetentionSettings(string alias, AddressRetentionSettings addressReplicationSettings, string updatedBy, bool requestedByOtherNode = false);
        (bool status, string message) EditAddressSchemaSettings(string alias, AddressSchemaSettings addressSchemaSettings, string updatedBy, bool requestedByOtherNode = false);

    }
}
