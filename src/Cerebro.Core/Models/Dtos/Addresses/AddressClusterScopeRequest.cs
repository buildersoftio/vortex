using Cerebro.Core.Models.Entities.Addresses;

namespace Cerebro.Core.Models.Dtos.Addresses
{
    public class AddressClusterScopeRequest
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
        AddressChangeRequested,
        AddressPartitionChangeRequested,
    }
}
