using Vortex.Core.Models.Containers;
using Vortex.Core.Models.Entities.Entries;

namespace Vortex.Core.Abstractions.Services.Orchestrations
{
    public interface IServerCoreStateManager
    {
        public void LoadAddressPartitionsInMemory(string addressAlias);
        public void LoadAddressPartitionsInMemory(int addressId);

        public void LoadApplicationSubscriptionsInMemory(int applicationId, string addressAlias, string subscriptionName);
        public void UnloadApplicationSubscriptionsFromMemory(int applicationId, int addressId, string subscriptionName);


        bool IsAddressPartitionsLoaded(string addressAlias);

        public void UnloadAddressPartitionsInMemory(string addressAlias);
        public void UnloadAddressPartitionsInMemory(int addressId);

        public void UpdatePartitionEntry(PartitionEntry partitionEntry);

        public AddressContainer GetAddressContainer(int addressId);
        public AddressContainer? GetAddressContainer(string addressAlias);
    }
}
