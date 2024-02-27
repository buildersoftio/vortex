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
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Models.Entities.Clients.Applications;
using Vortex.Core.Utilities.Ids;
using Vortex.Core.Models.BackgroundTimerRequests;
using Vortex.Core.Services.Routing.Background;

namespace Vortex.Core.Services.Orchestrations
{
    public class ServerCoreStateManager : SimpleBackgroundQueueServiceBase<PartitionEntry>, IServerCoreStateManager
    {
        private readonly ILogger<ServerCoreStateManager> _logger;
        private readonly IPartitionDataFactory _partitionDataFactory;
        private readonly IAddressRepository _addressRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IPartitionEntryService _partitionEntryService;
        private readonly ISubscriptionEntryService _subscriptionEntryService;

        private readonly NodeConfiguration _nodeConfiguration;

        private readonly IClusterStateRepository _clusterStateRepository;

        private readonly ConcurrentDictionary<int, AddressContainer> _inMemoryAddresses;

        // id is applicationId:addressId:subscriptionName
        private readonly ConcurrentDictionary<string, SubscriptionContainer> _inMemorySubscriptions;
        private readonly ConcurrentDictionary<string, TimedBackgroundServiceBase<SubscriptionEntryTimerRequest>> _subscriptionEntryBackgroundServices;


        public ServerCoreStateManager(ILogger<ServerCoreStateManager> logger,
            IPartitionDataFactory partitionDataFactory,
            IAddressRepository addressRepository,
            IApplicationRepository applicationRepository,
            IPartitionEntryService partitionEntryService,
            ISubscriptionEntryService subscriptionEntryService,
            NodeConfiguration nodeConfiguration,
            IClusterStateRepository clusterStateRepository)
        {
            _logger = logger;
            _partitionDataFactory = partitionDataFactory;
            _addressRepository = addressRepository;
            _applicationRepository = applicationRepository;
            _partitionEntryService = partitionEntryService;
            _subscriptionEntryService = subscriptionEntryService;

            _nodeConfiguration = nodeConfiguration;

            _clusterStateRepository = clusterStateRepository;
            _inMemoryAddresses = new ConcurrentDictionary<int, AddressContainer>();
            _inMemorySubscriptions = new ConcurrentDictionary<string, SubscriptionContainer>();

            _subscriptionEntryBackgroundServices = new ConcurrentDictionary<string, TimedBackgroundServiceBase<SubscriptionEntryTimerRequest>>();
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

        public void LoadApplicationSubscriptionsInMemory(int applicationId, string addressAlias, string subscriptionName)
        {
            var address = _addressRepository.GetAddressByAlias(addressAlias);
            if (address == null)
                return;

            var application = _applicationRepository.GetApplication(applicationId);
            if (application == null)
                return;

            LoadApplicationSubscriptions(application, address, subscriptionName);
        }

        private void LoadApplicationSubscriptions(Application application, Address address, string subscriptionName)
        {
            var subscriptions = _subscriptionEntryService.GetSubscriptionEntries(application.Id, address.Id);
            if (subscriptions.Count == 0)
            {
                _logger.LogInformation($"There is no subscription linked between application [{application.Name}] and address [{address.Name}] with name [{subscriptionName}]");
                return;
            }

            if (_inMemorySubscriptions.ContainsKey(ApplicationAddressIdHelper.ToApplicationAddressId(application.Id, address.Id, subscriptionName)))
            {
                _logger.LogInformation($"Subscription [{subscriptionName}] for application [{application.Name}] and address [{address.Name}] is already loaded");
                return;
            }

            var key = ApplicationAddressIdHelper.ToApplicationAddressId(application.Id, address.Id, subscriptionName);
            _inMemorySubscriptions.TryAdd(key, new SubscriptionContainer()
            {
                AddressName = address.Name,
                ApplicationName = application.Name,
                SubscriptionEntries = subscriptions
            });

            // initialize the background service
            var subscriptionBackgroundService = new SubscriptionEntryBackgroundService(_nodeConfiguration, _inMemorySubscriptions[key], _subscriptionEntryService);
            _subscriptionEntryBackgroundServices.TryAdd(key, subscriptionBackgroundService);
        }

        public void UnloadApplicationSubscriptionsFromMemory(int applicationId, int addressId, string subscriptionName)
        {
            var address = _addressRepository.GetAddressById(addressId);
            if (address == null)
                return;

            var application = _applicationRepository.GetApplication(applicationId);
            if (application == null)
                return;


            UnloadApplicationSubscriptions(application, address, subscriptionName);
        }

        private void UnloadApplicationSubscriptions(Application application, Address address, string subscriptionName)
        {
            var key = ApplicationAddressIdHelper.ToApplicationAddressId(application.Id, address.Id, subscriptionName);

            // TODO: find a way to stop the background time service
            //       even if we break it.
            if (_subscriptionEntryBackgroundServices.TryRemove(key, out _))
            {
                if (_inMemorySubscriptions.TryRemove(key, out _))
                    _logger.LogInformation($"Subscription [{subscriptionName}] connected to application [{application.Name}] and address [{address.Name}] is unloaded from memory");
            }
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

            _inMemoryAddresses.TryAdd(address.Id, new AddressContainer()
            {
                AddressAlias = address.Alias,
                AddressName = address.Name,
                PartitionEntries = partitions
            });

            foreach (var partition in partitions)
            {
                // load the data access now...

                var partitionDataService = new PartitionDataService(_partitionEntryService, _partitionDataFactory, address, partition);
                _inMemoryAddresses[address.Id].PartitionDataProcessors.TryAdd(partition.PartitionId,
                    new PartitionDataProcessor(_partitionEntryService, address, partition, partitionDataService, _nodeConfiguration, _clusterStateRepository));
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

        public bool IsAddressPartitionsLoaded(string addressAlias)
        {
            var address = _inMemoryAddresses.Values.Where(x => x.AddressAlias == addressAlias).FirstOrDefault();
            if (address != null)
                return true;

            return false;
        }

        public AddressContainer? GetAddressContainer(string addressAlias)
        {
            // This is bad for high volume of data ...

            var address = _inMemoryAddresses.Values
                .Where(x => x.AddressAlias == addressAlias).FirstOrDefault()!;

            return address;
        }
    }
}
