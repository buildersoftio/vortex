using Microsoft.Extensions.Logging;
using Vortex.Core.Abstractions.Repositories;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Entities.Entries;
using Vortex.Core.Repositories;

namespace Vortex.Core.Services.Entries
{
    public class SubscriptionEntryService : ISubscriptionEntryService
    {
        private readonly ILogger<SubscriptionEntryService> _logger;
        private readonly ISubscriptionEntryRepository _subscriptionEntryRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IApplicationRepository _applicationRepository;

        public SubscriptionEntryService(ILogger<SubscriptionEntryService> logger,
            ISubscriptionEntryRepository subscriptionEntryRepository,
            IAddressRepository addressRepository,
            IApplicationRepository applicationRepository)
        {
            _logger = logger;

            _subscriptionEntryRepository = subscriptionEntryRepository;
            _addressRepository = addressRepository;
            _applicationRepository = applicationRepository;
        }

        public bool CreateSubscriptionEntry(string subscriptionName, int applicationId, int addressId, int partitionId, string addressAlias, ConsumptionSettings consumptionSettings, string createdBy)
        {
            var subscriptionEntry = _subscriptionEntryRepository.GetSubscriptionEntry(applicationId, addressId, subscriptionName, partitionId);
            if (subscriptionEntry != null)
                return false;

            var application = _applicationRepository.GetApplication(applicationId);
            if (application == null)
                return false;

            subscriptionEntry = new SubscriptionEntry()
            {
                ConsumptionSettings = consumptionSettings,
                AddressAlias = addressAlias,
                AddressId = addressId,
                ApplicationId = applicationId,
                ApplicationName = application.Name,
                PartitionId = partitionId,
                ReadCommittedEntry = 0,
                SubscriptionName = subscriptionName,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            return _subscriptionEntryRepository.AddSubscriptionEntity(subscriptionEntry);
        }

        public bool DeleteSubscriptionEntries(string subscriptionName, int applicationId, int addressId)
        {
            var entries = _subscriptionEntryRepository.GetSubscriptionEntries(applicationId, addressId, subscriptionName);
            if (entries.Count == 0)
                return false;

            foreach (var entry in entries)
            {
                if (_subscriptionEntryRepository.DeleteSubscriptionEntity(entry) != true)
                    return false;
            }

            return true;
        }

        public bool DeleteSubscriptionEntries(string subscriptionName, string applicationName, string addressName)
        {
            var address = _addressRepository.GetAddressByName(addressName);
            if (address == null)
                return false;

            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return false;

            return DeleteSubscriptionEntries(subscriptionName, application.Id, address.Id);
        }

        public bool DeleteSubscriptionEntriesByAddress(int addressId)
        {

            var entries = _subscriptionEntryRepository.GetSubscriptionEntries(addressId);

            foreach (var entry in entries)
            {
                entry.IsActive = false;
                entry.IsDeleted = true;
                entry.UpdatedAt = DateTime.UtcNow;
                entry.UpdatedBy = "bg_system";

                if (_subscriptionEntryRepository.UpdateSubscriptionEntity(entry) != true)
                    return false;
            }

            return true;
        }

        public bool DeleteSubscriptionEntriesByApplication(int applicationId)
        {
            var entries = _subscriptionEntryRepository.GetSubscriptionEntriesByApplication(applicationId);

            foreach (var entry in entries)
            {
                entry.IsActive = false;
                entry.IsDeleted = true;
                entry.UpdatedAt = DateTime.UtcNow;
                entry.UpdatedBy = "bg_system";

                if (_subscriptionEntryRepository.UpdateSubscriptionEntity(entry) != true)
                    return false;
            }

            return true;
        }

        public List<SubscriptionEntry> GetSubscriptionEntries(string subscriptionName, string applicationName, string addressName)
        {
            var address = _addressRepository.GetAddressByName(addressName);
            if (address == null)
                return new List<SubscriptionEntry>();

            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return new List<SubscriptionEntry>();

            return _subscriptionEntryRepository.GetSubscriptionEntries(application.Id, address.Id, subscriptionName);
        }

        public List<SubscriptionEntry> GetSubscriptionEntries(string subscriptionName, int applicationId, int addressId)
        {
            return _subscriptionEntryRepository.GetSubscriptionEntries(applicationId, addressId, subscriptionName);
        }

        public List<SubscriptionEntry> GetSubscriptionEntries(int addressId, int partitionId)
        {
            return _subscriptionEntryRepository.GetSubscriptionEntries(addressId, partitionId);
        }

        public List<string> GetSubscriptions(int applicationId, int addressId)
        {
            return _subscriptionEntryRepository
                .GetSubscriptionEntries(applicationId, addressId)
                .GroupBy(e => e.SubscriptionName)
                   .Select(group => group.Key)
                   .ToList();
        }

        public bool UpdateSubscriptionEntries(SubscriptionEntry entry)
        {
            return _subscriptionEntryRepository
                 .UpdateSubscriptionEntity(entry);
        }
    }
}
