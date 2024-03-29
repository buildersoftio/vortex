﻿using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Models.BackgroundRequests;
using Vortex.Core.Models.Common.Addresses;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Dtos.Addresses;
using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Repositories;
using Microsoft.Extensions.Logging;
using Vortex.Core.Services.Entries;

namespace Vortex.Core.Services.ServerStates
{
    public class AddressService : IAddressService
    {
        private readonly ILogger<AddressService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly ISimpleBackgroundQueueService<AddressBackgroundRequest> _backgroundServerStateService;
        private readonly IBackgroundQueueService<AddressClusterScopeRequest> _backgroundAddressClusterService;
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly StorageDefaultConfiguration _storageDefaultConfiguration;
        private readonly IClusterStateRepository _clusterStateRepository;
        private readonly ISubscriptionEntryService _subscriptionEntryService;

        public AddressService(ILogger<AddressService> logger,
            IAddressRepository addressRepository,
            ISimpleBackgroundQueueService<AddressBackgroundRequest> backgroundServerStateService,
            IBackgroundQueueService<AddressClusterScopeRequest> backgroundAddressClusterService,
            NodeConfiguration nodeConfiguration,
            StorageDefaultConfiguration storageDefaultConfiguration,
            IClusterStateRepository clusterStateRepository,
            ISubscriptionEntryService subscriptionEntryService)
        {
            _logger = logger;
            _addressRepository = addressRepository;

            _backgroundServerStateService = backgroundServerStateService;
            _backgroundAddressClusterService = backgroundAddressClusterService;

            _nodeConfiguration = nodeConfiguration;
            _storageDefaultConfiguration = storageDefaultConfiguration;

            _clusterStateRepository = clusterStateRepository;
            _subscriptionEntryService = subscriptionEntryService;
        }

        public (bool status, string message) CreateAddress(AddressCreationRequest addressCreationRequest, string createdBy, bool requestedByOtherNode = false)
        {
            (var address, string message) = GetAddressByAliasAndName(addressCreationRequest.Alias, addressCreationRequest.Name);
            if (address != null)
                return (status: false, message: message);


            // checking the replication factor.
            if (addressCreationRequest.Settings.ReplicationSettings.ReplicationFactor > _clusterStateRepository.GetNodes().Count() + 1)
                return (status: false, message: $"Address [{addressCreationRequest.Name}] can not be created,  ReplicationFactor should be lower or equal with number of nodes connected in the cluster");

            if (addressCreationRequest.Settings.Scope == AddressScope.SingleScope && addressCreationRequest.Settings.ReplicationSettings.ReplicationFactor > 1)
                return (status: false, message: $"Address [{addressCreationRequest.Name}] can not be created, in a SingleScope address configuration, ReplicationFactor should be 1");


            address = new Address()
            {
                Name = addressCreationRequest.Name,
                Alias = addressCreationRequest.Alias,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                Settings = addressCreationRequest.Settings,
                Status = AddressStatuses.CreateAddressDirectory,
            };

            var result = _addressRepository.AddAddress(address);
            if (result == true)
            {
                // We are enqueueing the address for directory creation and rocks-db_partition and replication services

                _logger.LogInformation($"Address [{address.Name}] is created successfully");

                _backgroundServerStateService.EnqueueRequest(new AddressBackgroundRequest()
                {
                    Address = address,
                    IsRequestedFromOtherNode = requestedByOtherNode
                });

                return (true, $"Address [{addressCreationRequest.Name}] created successfully at [{address.Id}]");
            }

            return (false, $"Something went wrong, Address [{addressCreationRequest.Name}] isnot created");
        }

        public (bool status, string message) CreateDefaultAddress(AddressDefaultCreationRequest addressDefaultCreationRequest, string createdBy, bool requestedByOtherNode = false)
        {
            var defaultSettings = new AddressSettings()
            {
                EnforceSchemaValidation = false,
                Scope = addressDefaultCreationRequest.Scope,
                MessageIndexType = MessageIndexTypes.DAILY,
                PartitionSettings = new AddressPartitionSettings() { PartitionNumber = addressDefaultCreationRequest.PartitionNumber, PartitionThreadLimit = 1 },
                ReplicationSettings = new AddressReplicationSettings() { ReplicationFactor = addressDefaultCreationRequest.ReplicationFactor },
                RetentionSettings = new AddressRetentionSettings() { RetentionType = RetentionTypes.DELETE, TimeToLiveInMinutes = -1 },
                SchemaSettings = new AddressSchemaSettings(),
                StorageSettings = new AddressStorageSettings(_storageDefaultConfiguration),

            };

            var addressCreationRequest = new AddressCreationRequest()
            {
                Alias = addressDefaultCreationRequest.Alias,
                Name = addressDefaultCreationRequest.Name,
                Settings = defaultSettings
            };

            (bool status, string message) = CreateAddress(addressCreationRequest, createdBy, requestedByOtherNode);

            return (status, message);
        }

        public (bool status, string message) DeleteAddress(string alias, bool requestedByOtherNode = false)
        {
            var address = _addressRepository.GetAddressByAlias(alias);
            if (address == null)
                return (status: false, message: $"Address alias {alias} doesnot exists");

            if (_addressRepository.DeleteAddress(address))
            {

                // delete connections and subscriptions.
                _subscriptionEntryService.DeleteSubscriptionEntriesByAddress(address.Id);

                address.Status = AddressStatuses.DeletePartitions;
                _backgroundServerStateService.EnqueueRequest(new AddressBackgroundRequest() { Address = address });

                // in case of cluster scope, inform other nodes to create address
                if (address.Settings.Scope == AddressScope.ClusterScope && requestedByOtherNode != true)
                {

                    // TODO: figure it out how to remove partition data from the disk.
                    //..
                    //..

                    _backgroundAddressClusterService.EnqueueRequest(new AddressClusterScopeRequest()
                    {
                        AddressCreationRequest = new AddressCreationRequest() { Alias = address.Alias, Name = address.Name, Settings = address.Settings },
                        AddressClusterScopeRequestState = AddressClusterScopeRequestState.AddressDeletionRequested,
                        RequestedBy = "delete_sys"
                    });
                }

                return (status: true, message: $"Address [{address.Name}] has been removed successfully");
            }


            return (status: false, message: $"Something went wrong, address [{address.Name}] cannot be deleted");
        }

        public (bool status, string message) EditAddressPartitionSettings(string alias, AddressPartitionSettings addressPartitionSettings, string updatedBy, bool requestedByOtherNode = false)
        {
            var address = _addressRepository.GetAddressByAlias(alias);
            if (address == null)
                return (status: false, message: $"Address alias {alias} doesnot exists");

            if (addressPartitionSettings.PartitionNumber < address.Settings.PartitionSettings.PartitionNumber)
                return (status: false, message: $"New partition number cannot be smaller than existing partition number");

            address.Settings.PartitionSettings = addressPartitionSettings;
            address.Status = AddressStatuses.ChangePartitions;

            address.UpdatedAt = DateTime.UtcNow;
            address.UpdatedBy = updatedBy;

            if (_addressRepository.UpdateAddress(address))
            {
                // We are enqueueing the address for partition creation

                _logger.LogInformation($"Address [{address.Name}] partitions change at {addressPartitionSettings.PartitionNumber} requested");
                _logger.LogInformation($"Address [{address.Name}] initializing new partitions");

                _backgroundServerStateService.EnqueueRequest(new AddressBackgroundRequest() { Address = address, IsRequestedFromOtherNode = requestedByOtherNode });

                return (status: true, message: $"Address [{address.Name}] partition settings changed");
            }

            return (status: false, message: $"Something went wrong, address [{address.Name}] partition settings is not updated");
        }

        public (bool status, string message) EditAddressReplicationSettings(string alias, AddressReplicationSettings addressReplicationSettings, string updatedBy, bool requestedByOtherNode = false)
        {
            var address = _addressRepository.GetAddressByAlias(alias);
            if (address == null)
                return (status: false, message: $"Address alias {alias} doesnot exists");

            address.Settings.ReplicationSettings = addressReplicationSettings;
            address.UpdatedAt = DateTime.UtcNow;
            address.UpdatedBy = updatedBy;

            if (_addressRepository.UpdateAddress(address))
            {
                if (address.Settings.Scope == AddressScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundAddressClusterService.EnqueueRequest(new AddressClusterScopeRequest()
                    {
                        AddressCreationRequest = new AddressCreationRequest() { Alias = address.Alias, Name = address.Name, Settings = address.Settings },
                        AddressClusterScopeRequestState = AddressClusterScopeRequestState.AddressReplicationSettingsChangeRequested,
                        RequestedBy = updatedBy
                    });
                }

                return (status: true, message: $"Address [{address.Name}] replication settings changed");
            }

            return (status: false, message: $"Something went wrong, address [{address.Name}] replication settings is not updated");
        }

        public (bool status, string message) EditAddressRetentionSettings(string alias, AddressRetentionSettings addressReplicationSettings, string updatedBy, bool requestedByOtherNode = false)
        {
            var address = _addressRepository.GetAddressByAlias(alias);
            if (address == null)
                return (status: false, message: $"Address alias {alias} doesnot exists");

            if (addressReplicationSettings.RetentionType != address.Settings.RetentionSettings.RetentionType)
                return (status: false, message: $"Retention type cannot change after address is created");

            address.Settings.RetentionSettings = addressReplicationSettings;
            address.UpdatedAt = DateTime.UtcNow;
            address.UpdatedBy = updatedBy;

            if (_addressRepository.UpdateAddress(address))
            {
                // in case of cluster scope, inform other nodes
                if (address.Settings.Scope == AddressScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundAddressClusterService.EnqueueRequest(new AddressClusterScopeRequest()
                    {
                        AddressCreationRequest = new AddressCreationRequest() { Alias = address.Alias, Name = address.Name, Settings = address.Settings },
                        AddressClusterScopeRequestState = AddressClusterScopeRequestState.AddressRetentionSettingsChangeRequested,
                        RequestedBy = updatedBy
                    });
                }

                return (status: true, message: $"Address [{address.Name}] retention settings changed");
            }

            return (status: false, message: $"Something went wrong, address [{address.Name}] retention settings is not updated");
        }

        public (bool status, string message) EditAddressSchemaSettings(string alias, AddressSchemaSettings addressSchemaSettings, string updatedBy, bool requestedByOtherNode = false)
        {
            var address = _addressRepository.GetAddressByAlias(alias);
            if (address == null)
                return (status: false, message: $"Address alias {alias} doesnot exists");


            address.Settings.SchemaSettings = addressSchemaSettings;
            address.UpdatedAt = DateTime.UtcNow;
            address.UpdatedBy = updatedBy;

            if (_addressRepository.UpdateAddress(address))
            {
                // in case of cluster scope, inform other nodes
                if (address.Settings.Scope == AddressScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundAddressClusterService.EnqueueRequest(new AddressClusterScopeRequest()
                    {
                        AddressCreationRequest = new AddressCreationRequest() { Alias = address.Alias, Name = address.Name, Settings = address.Settings },
                        AddressClusterScopeRequestState = AddressClusterScopeRequestState.AddressSchemaSettingsChangeRequested,
                        RequestedBy = updatedBy
                    });
                }

                return (status: true, message: $"Address [{address.Name}] schema settings changed");
            }

            return (status: false, message: $"Something went wrong, address [{address.Name}] schema settings is not updated");
        }

        public (bool status, string message) EditAddressStorageSettings(string alias, AddressStorageSettings addressStorageSettings, string updatedBy, bool requestedByOtherNode = false)
        {
            var address = _addressRepository.GetAddressByAlias(alias);
            if (address == null)
                return (status: false, message: $"Address alias {alias} doesnot exists");


            address.Settings.StorageSettings = addressStorageSettings;
            address.UpdatedAt = DateTime.UtcNow;
            address.UpdatedBy = updatedBy;

            if (_addressRepository.UpdateAddress(address))
            {
                // in case of cluster scope, inform other nodes
                if (address.Settings.Scope == AddressScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundAddressClusterService.EnqueueRequest(new AddressClusterScopeRequest()
                    {
                        AddressCreationRequest = new AddressCreationRequest() { Alias = address.Alias, Name = address.Name, Settings = address.Settings },
                        AddressClusterScopeRequestState = AddressClusterScopeRequestState.AddressStorageSettingsChangeRequested,
                        RequestedBy = updatedBy
                    });
                }

                return (status: true, message: $"Address [{address.Name}] storage settings changed, it might take sometime for the update to take place");
            }

            return (status: false, message: $"Something went wrong, address [{address.Name}] storage settings is not updated");
        }


        public (AddressDto? address, string message) GetAddressByAlias(string alias)
        {
            var address = _addressRepository.GetAddressByAlias(alias);
            if (address == null)
                return (address: null, message: $"Address alias {alias} doesnot exists");

            return (address: new AddressDto(address), message: $"Address alias {alias} returned");
        }

        public (AddressDto? address, string message) GetAddressById(int addressId)
        {
            var address = _addressRepository.GetAddressById(addressId);
            if (address == null)
                return (address: null, message: $"Address with id {addressId} doesnot exists");

            return (address: new AddressDto(address), message: $"Address with id {addressId} returned");
        }

        public (AddressDto? address, string message) GetAddressByName(string name)
        {
            var address = _addressRepository.GetAddressByName(name);
            if (address == null)
                return (address: null, message: $"Address [{name}] doesnot exists");

            return (address: new AddressDto(address), message: $"Address [{name}] returned");
        }

        public (List<AddressDto>? addresses, string message) GetAddresses()
        {
            var addresses = _addressRepository
                .GetAddresses()
                .Select(a => new AddressDto(a))
                .ToList();

            return (addresses: addresses, message: "Addresses Returned");
        }

        public (bool status, string message) PromoteAddress(string alias, string updatedBy, bool requestedByOtherNode = false)
        {
            var address = _addressRepository.GetAddressByAlias(alias);
            if (address == null)
                return (status: false, message: $"Address alias {alias} doesnot exists");

            if (address.Settings.Scope == AddressScope.ClusterScope)
                return (status: false, message: $"Address [{address.Name}] is already promoted");

            address.Settings.Scope = AddressScope.ClusterScope;
            address.UpdatedAt = DateTimeOffset.UtcNow;
            address.UpdatedBy = updatedBy;

            if (_addressRepository.UpdateAddress(address) == true)
            {
                // in case of cluster scope, inform other nodes to create address
                if (address.Settings.Scope == AddressScope.ClusterScope && requestedByOtherNode != true)
                {
                    // Sending Address Creation Request in case of promoting, that means the address should not exists in other nodes.
                    _backgroundAddressClusterService.EnqueueRequest(new AddressClusterScopeRequest()
                    {
                        AddressCreationRequest = new AddressCreationRequest() { Alias = address.Alias, Name = address.Name, Settings = address.Settings },
                        AddressClusterScopeRequestState = AddressClusterScopeRequestState.AddressCreationRequested,
                        RequestedBy = updatedBy
                    });
                }

                return (status: true, message: $"Address [{address.Name}] has been removed successfully");
            }

            return (status: false, message: $"Something went wrong, address [{address.Name}] cannot be be promoted to ClusterScope");
        }

        private (Address? address, string message) GetAddressByAliasAndName(string alias, string name)
        {
            var address = _addressRepository.GetAddressByAlias(alias);
            if (address != null)
                return (address: address, message: $"Address alias {alias} already exists");
            address = _addressRepository.GetAddressByName(name);
            if (address != null)
                return (address: address, message: $"Address name {name} already exists");

            return (null, "Address doesnot exists");
        }
    }
}
