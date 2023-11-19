using Vortex.Core.Models.Common.Addresses;
using Vortex.Core.Models.Entities.Addresses;

namespace Vortex.Core.Models.Dtos.Addresses
{
    public class AddressDto
    {
        public string Alias { get; set; }
        public string Name { get; set; } // name will be in rules like "root/something" or "something"...

        public AddressStatuses Status { get; set; }
        public AddressSettings Settings { get; set; }

        public AddressDto() { }

        public AddressDto(Address address)
        {
            Alias = address.Alias;
            Name = address.Name;
            Status = address.Status;
            Settings = address.Settings;
        }
    }
}
