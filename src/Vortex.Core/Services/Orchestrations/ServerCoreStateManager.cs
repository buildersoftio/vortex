using Cerebro.Core.Abstractions.Background;
using Cerebro.Core.Abstractions.Factories;
using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.Abstractions.Services.Orchestrations;
using Cerebro.Core.Models.Containers;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Models.Entities.Entries;
using Cerebro.Core.Repositories;
using Cerebro.Core.Services.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Cerebro.Core.Services.Orchestrations
{
    public class ServerCoreStateManager : SimpleBackgroundQueueServiceBase<PartitionEntry>, IServerCoreStateManager
    {
        private readonly ILogger<ServerCoreStateManager> _logger;
        private readonly IPartitionDataFactory _partitionDataFactory;
        private readonly IAddressRepository _addressRepository;
        private readonly IPartitionEntryService _partitionEntryService;

        private readonly ConcurrentDictionary<int, AddressContainer> _inMemoryAddresses;


        public ServerCoreStateManager(ILogger<ServerCoreStateManager> logger,
            IPartitionDataFactory partitionDataFactory,
            IAddressRepository addressRepository,
            IPartitionEntryService partitionEntryService)
        {
            _logger = logger;
            _partitionDataFactory = partitionDataFactory;
            _addressRepository = addressRepository;
            _partitionEntryService = partitionEntryService;

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

            _inMemoryAddresses.TryAdd(address.Id, new AddressContainer() { AddressAlias = address.Alias, AddressName = address.Name, PartitionEntries = partitions });
            foreach (var partition in partitions)
            {
                // load the data access now...
                _inMemoryAddresses[address.Id].PartitionDataServices.TryAdd(partition.PartitionId,
                    new PartitionDataService(_partitionEntryService, _partitionDataFactory, address, partition));
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
    }
}
