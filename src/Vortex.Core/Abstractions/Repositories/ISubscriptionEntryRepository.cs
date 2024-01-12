using Vortex.Core.Models.Entities.Entries;

namespace Vortex.Core.Abstractions.Repositories
{
    public interface ISubscriptionEntryRepository
    {
        bool AddSubscriptionEntity(SubscriptionEntry subscriptionEntity);
        bool UpdateSubscriptionEntity(SubscriptionEntry subscriptionEntity);
        bool DeleteSubscriptionEntity(SubscriptionEntry subscriptionEntity);

        SubscriptionEntry GetSubscriptionEntry(int applicationId, int addressId, string subscriptionName, int partitionId);

        List<SubscriptionEntry> GetSubscriptionEntry(int addressId, int partitionId);

        List<SubscriptionEntry> GetSubscriptionEntriesByApplication(int applicationId, bool returnDeleted = false);
        List<SubscriptionEntry> GetSubscriptionEntries(int applicationId, int addressId, string subscriptionName, bool returnDeleted = false);
        List<SubscriptionEntry> GetSubscriptionEntries(int applicationId, int addressId, bool returnDeleted = false);
        List<SubscriptionEntry> GetSubscriptionEntries(int addressId, bool returnDeleted = false);
        List<SubscriptionEntry> GetSubscriptionEntries(string addressAlias, bool returnDeleted = false);
    }
}
