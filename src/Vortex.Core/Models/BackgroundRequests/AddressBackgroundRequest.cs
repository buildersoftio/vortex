using Cerebro.Core.Models.Entities.Addresses;

namespace Cerebro.Core.Models.BackgroundRequests
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
