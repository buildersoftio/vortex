using Cerebro.Core.Models.Entities.Addresses;

namespace Cerebro.Core.Repositories
{
    public interface IAddressRepository
    {
        bool AddAddress(Address address);
        bool DeleteAddress(int addressId);
        bool DeleteAddress(string addressName);
        bool UpdateAddress(int addressId, Address address);
        bool UpdateAddress(string addressName,  Address address);
        Address GetAddress(int addressId);
        Address GetAddress(string addressName);

        bool AddAddressSettings(int addressId, AddressSettings addressSettings);
        bool DeleteAddressSettings(int addressId);
        bool UpdateAddressSettings(int addressId, AddressSettings addressSettings);

        AddressSettings GetAddressSettings(int addressId);
    }
}
