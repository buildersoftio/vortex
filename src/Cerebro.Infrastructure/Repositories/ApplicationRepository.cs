using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Entities.Clients.Applications;
using Cerebro.Core.Repositories;
using Cerebro.Infrastructure.DataAccess.ApplicationStateStore;
using Microsoft.Extensions.Logging;

namespace Cerebro.Infrastructure.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ILogger<ApplicationRepository> _logger;
        private readonly ApplicationStateStoreDbContext _applicationStateStoreDbContext;

        public ApplicationRepository(ILogger<ApplicationRepository> logger, ApplicationStateStoreDbContext applicationStateStoreDbContext)
        {
            _logger = logger;
            _applicationStateStoreDbContext = applicationStateStoreDbContext;
        }

        public bool AddApplication(Application application)
        {
            var id = _applicationStateStoreDbContext.Applications!.Insert(application);
            if (id != 0)
                return true;

            return false;
        }

        public bool AddApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection)
        {
            var id = _applicationStateStoreDbContext.ApplicationAddressConnections!.Insert(applicationAddressConnection);
            if (id != 0)
                return true;

            return false;
        }

        public bool AddApplicationPermission(ApplicationPermission applicationPermission)
        {
            var id = _applicationStateStoreDbContext.ApplicationPermissions!.Insert(applicationPermission);
            if (id != 0)
                return true;

            return false;
        }

        public bool AddApplicationToken(ApplicationToken applicationToken)
        {
            var id = _applicationStateStoreDbContext.ApplicationTokens!.Insert(applicationToken);
            if (id != 0)
                return true;

            return false;
        }

        public bool DeleteApplication(Application application)
        {
            return _applicationStateStoreDbContext
                .Applications!
                .Delete(application.Id);
        }

        public bool DeleteApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection)
        {
            return _applicationStateStoreDbContext
                .ApplicationAddressConnections!
                .Delete(applicationAddressConnection.Id);
        }

        public bool DeleteApplicationPermission(ApplicationPermission applicationPermission)
        {
            return _applicationStateStoreDbContext
                .ApplicationPermissions!
                .Delete(applicationPermission.ApplicationId);
        }

        public bool DeleteApplicationToken(ApplicationToken applicationToken)
        {
            return _applicationStateStoreDbContext
                 .ApplicationTokens!
                 .Delete(applicationToken.Id);
        }

        public Application? GetApplication(int applicationId)
        {
            return _applicationStateStoreDbContext
                .Applications!
                .FindById(applicationId);
        }

        public Application? GetApplication(string applicationName)
        {
            return _applicationStateStoreDbContext
                .Applications!
                .Query().Where(x => x.Name == applicationName)
                .FirstOrDefault();
        }

        public ApplicationAddressConnection? GetApplicationAddressConnection(int applicationId, int addressId, ApplicationConnectionTypes applicationConnectionTypes)
        {
            return _applicationStateStoreDbContext
                .ApplicationAddressConnections!
                .Query()
                .Where(x => x.ApplicationId == applicationId && x.AddressId == addressId && x.ApplicationConnectionType == applicationConnectionTypes)
                .FirstOrDefault();
        }

        public List<ApplicationAddressConnection>? GetApplicationAddressConnectionsByAddress(int addressId)
        {
            return _applicationStateStoreDbContext
                .ApplicationAddressConnections!
                .Query()
                .Where(x => x.AddressId == addressId)
                .ToList();
        }

        public List<ApplicationAddressConnection>? GetApplicationAddressConnectionsByApplication(int applicationId)
        {
            return _applicationStateStoreDbContext
                .ApplicationAddressConnections!
                .Query()
                .Where(x => x.ApplicationId == applicationId)
                .ToList();
        }

        public ApplicationPermission GetApplicationPermission(int applicationId)
        {
            return _applicationStateStoreDbContext.ApplicationPermissions!
                .Query().Where(x => x.ApplicationId == applicationId)
                .FirstOrDefault();
        }

        public List<Application> GetApplications()
        {
            return _applicationStateStoreDbContext
                .Applications!
                .FindAll()
                .ToList();
        }

        public ApplicationToken? GetApplicationToken(int applicationId, string hashedSecret)
        {
            return _applicationStateStoreDbContext
                .ApplicationTokens!
                .Query().Where(x => x.ApplicationId == applicationId && x.HashedSecret == hashedSecret)
                .FirstOrDefault();
        }

        public ApplicationToken? GetApplicationToken(int applicationId, Guid id)
        {
            return _applicationStateStoreDbContext
                .ApplicationTokens!
                .Query().Where(x => x.ApplicationId == applicationId && x.Id == id)
                .FirstOrDefault();
        }

        public ApplicationToken? GetApplicationToken(Guid key, string hashedSecret)
        {
            return _applicationStateStoreDbContext
                  .ApplicationTokens!
                  .Query().Where(x => x.HashedSecret == hashedSecret && x.Id == key)
                  .FirstOrDefault();
        }

        public List<ApplicationToken> GetApplicationTokens(int applicationId)
        {
            return _applicationStateStoreDbContext
                  .ApplicationTokens!
                  .Query().Where(x => x.ApplicationId == applicationId)
                  .ToList();
        }

        public bool UpdateApplication(Application application)
        {
            return _applicationStateStoreDbContext
                .Applications!
                .Update(application);
        }

        public bool UpdateApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection)
        {
            return _applicationStateStoreDbContext
                .ApplicationAddressConnections!
                .Update(applicationAddressConnection);
        }

        public bool UpdateApplicationPermission(ApplicationPermission applicationPermission)
        {
            return _applicationStateStoreDbContext
                .ApplicationPermissions!
                .Update(applicationPermission);
        }

        public bool UpdateApplicationToken(ApplicationToken applicationToken)
        {
            return _applicationStateStoreDbContext
                .ApplicationTokens!
                .Update(applicationToken);
        }
    }
}
