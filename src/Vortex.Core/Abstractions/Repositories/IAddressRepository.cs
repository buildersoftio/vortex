using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Models.Entities.Clients.Applications;

namespace Vortex.Core.Repositories
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

        List<ClientConnection> GetClientConnectionsByApplicationId(int applicationId);
        List<ClientConnection> GetClientConnectionsByAddressId(int addressId);
    }
}
