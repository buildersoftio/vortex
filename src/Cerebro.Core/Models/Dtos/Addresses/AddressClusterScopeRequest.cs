using Cerebro.Core.Abstractions.Background;

namespace Cerebro.Core.Models.Dtos.Addresses
{
    public class AddressClusterScopeRequest : RequestBase
    {
        public AddressCreationRequest AddressCreationRequest { get; set; }
        public AddressClusterScopeRequestState AddressClusterScopeRequestState { get; set; }

        public string RequestedBy { get; set; }

        public AddressClusterScopeRequest()
        {
            AddressClusterScopeRequestState = AddressClusterScopeRequestState.AddressCreationRequested;
        }
    }

    public enum AddressClusterScopeRequestState
    {
        AddressCreationRequested,
        AddressDeletionRequested,
        AddressPartitionChangeRequested,
        AddressReplicationSettingsChangeRequested,
        AddressRetentionSettingsChangeRequested,
        AddressSchemaSettingsChangeRequested,
        AddressStorageSettingsChangeRequested,
    }
}
