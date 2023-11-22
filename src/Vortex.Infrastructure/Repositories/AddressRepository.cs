using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Models.Entities.Clients.Applications;
using Vortex.Core.Repositories;
using Vortex.Infrastructure.DataAccess.ServerStateStore;
using Microsoft.Extensions.Logging;

namespace Vortex.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ILogger<AddressRepository> _logger;
        private readonly ServerStateStoreDbContext _serverStateStoreDbContext;

        public AddressRepository(ILogger<AddressRepository> logger, ServerStateStoreDbContext serverStateStoreDbContext)
        {
            _logger = logger;
            _serverStateStoreDbContext = serverStateStoreDbContext;
        }

        public bool AddAddress(Address address)
        {
            var id = _serverStateStoreDbContext.Addresses!.Insert(address);
            if (id != 0)
                return true;

            return false;
        }

        public bool DeleteAddress(Address address)
        {
            return _serverStateStoreDbContext
                .Addresses!
                .Delete(address.Id);
        }

        public Address? GetAddressByAlias(string addressAlias)
        {
            return _serverStateStoreDbContext
                .Addresses!
                .Query()
                .Where(x => x.Alias == addressAlias).FirstOrDefault();
        }

        public Address? GetAddressById(int addressId)
        {
            return _serverStateStoreDbContext
                .Addresses!
                .FindById(addressId);
        }

        public Address? GetAddressByName(string addressName)
        {
            return _serverStateStoreDbContext
                .Addresses!
                .Query()
                .Where(x => x.Name == addressName).FirstOrDefault();
        }

        public List<Address> GetAddresses()
        {
            return _serverStateStoreDbContext
              .Addresses!
              .Query()
              .ToList();
        }

        public List<ClientConnection> GetClientConnectionsByAddressId(int addressId)
        {
            return _serverStateStoreDbContext
                .ClientConnections!
                .Query()
                .Where(x=>x.AddressId == addressId).ToList();
        }

        public List<ClientConnection> GetClientConnectionsByApplicationId(int applicationId)
        {
            return _serverStateStoreDbContext
               .ClientConnections!
               .Query()
               .Where(x => x.ApplicationId == applicationId).ToList();
        }

        public bool UpdateAddress(Address address)
        {
            return _serverStateStoreDbContext
                .Addresses!
                .Update(address);
        }
    }
}
