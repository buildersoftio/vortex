using Vortex.Core.Models.Entities.Addresses;

namespace Vortex.Core.Models.BackgroundRequests
{
    public class AddressBackgroundRequest
    {
        public Address Address { get; set; }

        public bool IsRequestedFromOtherNode { get; set; }

        public AddressBackgroundRequest()
        {
            IsRequestedFromOtherNode = false;
        }
    }
}
