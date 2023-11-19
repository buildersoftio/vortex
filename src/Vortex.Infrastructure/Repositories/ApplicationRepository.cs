using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Entities.Clients.Applications;
using Cerebro.Core.Repositories;
using Cerebro.Infrastructure.DataAccess.ServerStateStore;
using Microsoft.Extensions.Logging;

namespace Cerebro.Infrastructure.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ILogger<ApplicationRepository> _logger;
        private readonly ServerStateStoreDbContext _serverStateStoreDbContext;

        public ApplicationRepository(ILogger<ApplicationRepository> logger, ServerStateStoreDbContext serverStateStoreDbContext)
        {
            _logger = logger;
            _serverStateStoreDbContext = serverStateStoreDbContext;
        }

        public bool AddApplication(Application application)
        {
            var id = _serverStateStoreDbContext.Applications!.Insert(application);
            if (id != 0)
                return true;

            return false;
        }

        public bool AddApplicationAddressConnection(ClientConnection applicationAddressConnection)
        {
            var id = _serverStateStoreDbContext.ClientConnections!.Insert(applicationAddressConnection);
            if (id != 0)
                return true;

            return false;
        }

        public bool AddApplicationPermission(ApplicationPermission applicationPermission)
        {
            var id = _serverStateStoreDbContext.ApplicationPermissions!.Insert(applicationPermission);
            if (id != 0)
                return true;

            return false;
        }

        public bool AddApplicationToken(ApplicationToken applicationToken)
        {
            var id = _serverStateStoreDbContext.ApplicationTokens!.Insert(applicationToken);
            if (id != 0)
                return true;

            return false;
        }

        public bool DeleteApplication(Application application)
        {
            return _serverStateStoreDbContext
                .Applications!
                .Delete(application.Id);
        }

        public bool DeleteApplicationAddressConnection(ClientConnection applicationAddressConnection)
        {
            return _serverStateStoreDbContext
                .ClientConnections!
                .Delete(applicationAddressConnection.Id);
        }

        public bool DeleteApplicationPermission(ApplicationPermission applicationPermission)
        {
            return _serverStateStoreDbContext
                .ApplicationPermissions!
                .Delete(applicationPermission.ApplicationId);
        }

        public bool DeleteApplicationToken(ApplicationToken applicationToken)
        {
            return _serverStateStoreDbContext
                 .ApplicationTokens!
                 .Delete(applicationToken.Id);
        }

        public Application? GetApplication(int applicationId)
        {
            return _serverStateStoreDbContext
                .Applications!
                .FindById(applicationId);
        }

        public Application? GetApplication(string applicationName)
        {
            return _serverStateStoreDbContext
                .Applications!
                .Query().Where(x => x.Name == applicationName)
                .FirstOrDefault();
        }

        public ClientConnection? GetClientConnection(int applicationId, int addressId, ApplicationConnectionTypes applicationConnectionTypes)
        {
            return _serverStateStoreDbContext
                .ClientConnections!
                .Query()
                .Where(x => x.ApplicationId == applicationId && x.AddressId == addressId && x.ApplicationConnectionType == applicationConnectionTypes)
                .FirstOrDefault();
        }

        public List<ClientConnection>? GetClientConnectionsByAddress(int addressId)
        {
            return _serverStateStoreDbContext
                .ClientConnections!
                .Query()
                .Where(x => x.AddressId == addressId)
                .ToList();
        }

        public List<ClientConnection>? GetClientConnectionsByApplication(int applicationId)
        {
            return _serverStateStoreDbContext
                .ClientConnections!
                .Query()
                .Where(x => x.ApplicationId == applicationId)
                .ToList();
        }

        public ApplicationPermission GetApplicationPermission(int applicationId)
        {
            return _serverStateStoreDbContext.ApplicationPermissions!
                .Query().Where(x => x.ApplicationId == applicationId)
                .FirstOrDefault();
        }

        public List<Application> GetApplications(bool sendSoftDeleted = true)
        {
            if (sendSoftDeleted != true)
                return _serverStateStoreDbContext
                        .Applications!.Query()
                        .Where(x => x.IsDeleted != true).ToList();


            return _serverStateStoreDbContext
                    .Applications!.FindAll()
                    .ToList();
        }

        public List<Application> GetActiveApplications()
        {
            return _serverStateStoreDbContext
           .Applications!.Query()
           .Where(x => x.IsDeleted != true && x.IsActive == true).ToList();
        }

        public ApplicationToken? GetApplicationToken(int applicationId, string hashedSecret)
        {
            return _serverStateStoreDbContext
                .ApplicationTokens!
                .Query().Where(x => x.ApplicationId == applicationId && x.HashedSecret == hashedSecret)
                .FirstOrDefault();
        }

        public ApplicationToken? GetApplicationToken(int applicationId, Guid id)
        {
            return _serverStateStoreDbContext
                .ApplicationTokens!
                .Query().Where(x => x.ApplicationId == applicationId && x.Id == id)
                .FirstOrDefault();
        }

        public ApplicationToken? GetApplicationToken(Guid key, string hashedSecret)
        {
            return _serverStateStoreDbContext
                  .ApplicationTokens!
                  .Query().Where(x => x.HashedSecret == hashedSecret && x.Id == key)
                  .FirstOrDefault();
        }

        public List<ApplicationToken> GetApplicationTokens(int applicationId)
        {
            return _serverStateStoreDbContext
                  .ApplicationTokens!
                  .Query().Where(x => x.ApplicationId == applicationId)
                  .ToList();
        }

        public bool UpdateApplication(Application application)
        {
            return _serverStateStoreDbContext
                .Applications!
                .Update(application);
        }

        public bool UpdateApplicationAddressConnection(ClientConnection applicationAddressConnection)
        {
            return _serverStateStoreDbContext
                .ClientConnections!
                .Update(applicationAddressConnection);
        }

        public bool UpdateApplicationPermission(ApplicationPermission applicationPermission)
        {
            return _serverStateStoreDbContext
                .ApplicationPermissions!
                .Update(applicationPermission);
        }

        public bool UpdateApplicationToken(ApplicationToken applicationToken)
        {
            return _serverStateStoreDbContext
                .ApplicationTokens!
                .Update(applicationToken);
        }
    }
}
