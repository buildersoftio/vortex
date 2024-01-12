using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Entities.Entries;

namespace Vortex.Core.Abstractions.Services
{
    public interface ISubscriptionEntryService
    {
        bool CreateSubscriptionEntry(string subscriptionName, int applicationId, int addressId, int partitionId, string addressAlias, ConsumptionSettings consumptionSettings, string createdBy);
        bool DeleteSubscriptionEntries(string subscriptionName, int applicationId, int addressId);
        bool DeleteSubscriptionEntries(string subscriptionName, string applicationName, string addressName);
        bool DeleteSubscriptionEntriesByApplication(int applicationId);
        bool DeleteSubscriptionEntriesByAddress(int addressId);

        List<SubscriptionEntry> GetSubscriptionEntries(string subscriptionName, string applicationName, string addressName);
        List<SubscriptionEntry> GetSubscriptionEntries(string subscriptionName, int applicationId, int addressId);

        List<SubscriptionEntry> GetSubscriptionEntries(int addressId, int partitionId);

        List<string> GetSubscriptions(int applicationId, int addressId);
    }
}
