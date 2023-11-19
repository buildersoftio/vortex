using Vortex.Core.Abstractions.Background;

namespace Vortex.Core.Models.Dtos.Addresses
{
    public class AddressClusterScopeRequest : RequestBase
    {
        public AddressCreationRequest AddressCreationRequest { get; set; }
        public AddressClusterScopeRequestState AddressClusterScopeRequestState { get; set; }

        public string RequestedBy { get; set; }

        public bool IsRequestedFromOtherNode { get; set; }

        public AddressClusterScopeRequest()
        {
            AddressClusterScopeRequestState = AddressClusterScopeRequestState.AddressCreationRequested;
            IsRequestedFromOtherNode = false;
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
