using Cerebro.Core.Models.Dtos.Addresses;

namespace Cerebro.Core.Abstractions.Services
{
    public interface IAddressService
    {
        bool CreateAddress(AddressDto addressDto);
        bool EditAddress(string addressName, AddressDto addressDto);
        bool DeleteAddress(string addressName);

        AddressDto GetAddress(string addressName);
        List<AddressDto> GetAddresses();
        List<AddressDto> GetAddresses(string addressNameFilter);
    }
}
