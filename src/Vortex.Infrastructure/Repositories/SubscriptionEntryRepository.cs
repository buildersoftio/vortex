using Microsoft.Extensions.Logging;
using System;
using Vortex.Core.Abstractions.Repositories;
using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Models.Entities.Entries;
using Vortex.Infrastructure.DataAccess.IndexesState;

namespace Vortex.Infrastructure.Repositories
{
    public class SubscriptionEntryRepository : ISubscriptionEntryRepository
    {
        private readonly ILogger<SubscriptionEntryRepository> _logger;
        private readonly IndexCatalogDbContext _indexCatalogDbContext;

        public SubscriptionEntryRepository(ILogger<SubscriptionEntryRepository> logger, IndexCatalogDbContext indexCatalogDbContext)
        {
            _logger = logger;
            _indexCatalogDbContext = indexCatalogDbContext;
        }

        public bool AddSubscriptionEntity(SubscriptionEntry subscriptionEntity)
        {

            var id = _indexCatalogDbContext.SubscriptionEntries!.Insert(subscriptionEntity);
            if (id != 1)
                return true;

            return false;
        }

        public bool DeleteSubscriptionEntity(SubscriptionEntry subscriptionEntity)
        {
            return _indexCatalogDbContext.SubscriptionEntries!.Delete(subscriptionEntity.Id);
        }

        public bool UpdateSubscriptionEntity(SubscriptionEntry subscriptionEntity)
        {
            return _indexCatalogDbContext
                .SubscriptionEntries!
                .Update(subscriptionEntity);
        }

        public List<SubscriptionEntry> GetSubscriptionEntries(int applicationId, int addressId, string subscriptionName, bool returnDeleted = false)
        {
            return _indexCatalogDbContext.SubscriptionEntries!.Query()
                .Where(x => x.ApplicationId == applicationId && x.AddressId == addressId && x.SubscriptionName == subscriptionName && x.IsDeleted == returnDeleted)
                .ToList();
        }

        public List<SubscriptionEntry> GetSubscriptionEntries(int applicationId, int addressId, bool returnDeleted = false)
        {
            return _indexCatalogDbContext.SubscriptionEntries!.Query()
                .Where(x => x.ApplicationId == applicationId && x.AddressId == addressId && x.IsDeleted == returnDeleted)
                .ToList();
        }

        public List<SubscriptionEntry> GetSubscriptionEntries(int addressId, bool returnDeleted = false)
        {
            return _indexCatalogDbContext.SubscriptionEntries!.Query()
                .Where(x => x.AddressId == addressId && x.IsDeleted == returnDeleted)
                .ToList();
        }

        public List<SubscriptionEntry> GetSubscriptionEntries(string addressAlias, bool returnDeleted = false)
        {
            return _indexCatalogDbContext.SubscriptionEntries!.Query()
                .Where(x => x.AddressAlias == addressAlias && x.IsDeleted == returnDeleted)
                .ToList();
        }

        public List<SubscriptionEntry> GetSubscriptionEntriesByApplication(int applicationId, bool returnDeleted = false)
        {
            return _indexCatalogDbContext.SubscriptionEntries!.Query()
               .Where(x => x.ApplicationId == applicationId && x.IsDeleted == returnDeleted)
               .ToList();
        }

        public List<SubscriptionEntry> GetSubscriptionEntry(int addressId, int partitionId)
        {
            return _indexCatalogDbContext.SubscriptionEntries!.Query()
                .Where(x => x.AddressId == addressId && x.PartitionId == partitionId)
                .ToList();
        }

        public SubscriptionEntry GetSubscriptionEntry(int applicationId, int addressId, string subscriptionName, int partitionId)
        {
            return _indexCatalogDbContext.SubscriptionEntries!.Query()
                .Where(x => x.ApplicationId == applicationId && x.AddressId == addressId && x.SubscriptionName == subscriptionName && x.PartitionId == partitionId)
                .FirstOrDefault();
        }
    }
}
