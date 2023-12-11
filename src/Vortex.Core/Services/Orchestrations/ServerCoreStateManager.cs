using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Factories;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Abstractions.Services.Orchestrations;
using Vortex.Core.Models.Containers;
using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Models.Entities.Entries;
using Vortex.Core.Repositories;
using Vortex.Core.Services.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Vortex.Core.Models.Configurations;

namespace Vortex.Core.Services.Orchestrations
{
    public class ServerCoreStateManager : SimpleBackgroundQueueServiceBase<PartitionEntry>, IServerCoreStateManager
    {
        private readonly ILogger<ServerCoreStateManager> _logger;
        private readonly IPartitionDataFactory _partitionDataFactory;
        private readonly IAddressRepository _addressRepository;
        private readonly IPartitionEntryService _partitionEntryService;
        private readonly NodeConfiguration _nodeConfiguration;

        private readonly ConcurrentDictionary<int, AddressContainer> _inMemoryAddresses;


        public ServerCoreStateManager(ILogger<ServerCoreStateManager> logger,
            IPartitionDataFactory partitionDataFactory,
            IAddressRepository addressRepository,
            IPartitionEntryService partitionEntryService,
            NodeConfiguration nodeConfiguration)
        {
            _logger = logger;
            _partitionDataFactory = partitionDataFactory;
            _addressRepository = addressRepository;
            _partitionEntryService = partitionEntryService;
            _nodeConfiguration = nodeConfiguration;

            _inMemoryAddresses = new ConcurrentDictionary<int, AddressContainer>();
        }

        public void LoadAddressPartitionsInMemory(string addressAlias)
        {
            var address = _addressRepository.GetAddressByAlias(addressAlias);
            if (address == null)
                return;

            LoadPartitionDataServices(address);
        }

        public void LoadAddressPartitionsInMemory(int addressId)
        {
            var address = _addressRepository.GetAddressById(addressId);
            if (address == null)
                return;

            LoadPartitionDataServices(address);
        }

        public void UnloadAddressPartitionsInMemory(string addressAlias)
        {
            var address = _addressRepository.GetAddressByAlias(addressAlias);
            if (address == null)
                return;

            UnloadPartitionDataServices(address);
        }

        public void UnloadAddressPartitionsInMemory(int addressId)
        {
            var address = _addressRepository.GetAddressById(addressId);
            if (address == null)
                return;

            UnloadPartitionDataServices(address);
        }

        private void LoadPartitionDataServices(Address address)
        {
            var partitions = _partitionEntryService.GetPartitionEntries(address.Id);

            if (_inMemoryAddresses.ContainsKey(address.Id))
            {

                _logger.LogInformation($"Address [{address.Name}] partitions are already loaded");
                return;
            }

            _inMemoryAddresses.TryAdd(address.Id, new AddressContainer() { AddressAlias = address.Alias, AddressName = address.Name, PartitionEntries = partitions });
            foreach (var partition in partitions)
            {
                // load the data access now...

                var partitionDataService = new PartitionDataService(_partitionEntryService, _partitionDataFactory, address, partition);
                _inMemoryAddresses[address.Id].PartitionDataProcessors.TryAdd(partition.PartitionId,
                    new PartitionDataProcessor(_partitionEntryService, address, partition, partitionDataService, _nodeConfiguration));
            }

            _logger.LogInformation($"Address [{address.Name}] partitions are loaded and ready");
        }

        private void UnloadPartitionDataServices(Address address)
        {
            if (_inMemoryAddresses.TryRemove(address.Id, out _))
                _logger.LogInformation($"Address [{address.Name}] partitions are unloaded successfully");
        }


        // Background service that updates the states of indexes.
        public override void Handle(PartitionEntry request)
        {
            try
            {
                _partitionEntryService.UpdatePartitionEntry(request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on updating partition [{request.PartitionId}] at address [{request.AddressAlias}], details {ex.Message}");
            }
        }

        public void UpdatePartitionEntry(PartitionEntry partitionEntry)
        {
            // Enqueueing the Partition Entry request to background service.
            EnqueueRequest(partitionEntry);
        }

        public AddressContainer GetAddressContainer(int addressId)
        {
            return _inMemoryAddresses[addressId];
        }
    }
}
