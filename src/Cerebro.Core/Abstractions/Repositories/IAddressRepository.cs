using Cerebro.Core.Models.Entities.Addresses;

namespace Cerebro.Core.Repositories
{
    public interface IAddressRepository
    {
        bool AddAddress(Address address);
        bool UpdateAddress(Address address);
        bool DeleteAddress(Address address);

        Address? GetAddressById(int addressId);
        Address? GetAddressByAlias(string addressAlias);
        Address? GetAddressByName(string addressName);
        List<Address> GetAddresses();
    }
}
