using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Models.Entities.Clients.Applications;
using Cerebro.Core.Repositories;
using Cerebro.Infrastructure.DataAccess.ApplicationStateStore;
using Microsoft.Extensions.Logging;

namespace Cerebro.Infrastructure.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ILogger<ApplicationRepository> _logger;
        private ApplicationStateStoreDbContext appStateStoreDbContext;
        public ApplicationRepository(ILogger<ApplicationRepository> logger)
        {
            _logger = logger;

            appStateStoreDbContext = new ApplicationStateStoreDbContext();
            appStateStoreDbContext.Database.EnsureCreated();
        }

        public bool AddApplication(Application application)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .Applications
                    .Add(application);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool AddApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationAddressConnections
                    .Add(applicationAddressConnection);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool AddApplicationPermission(ApplicationPermission applicationPermission)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationPermissions
                    .Add(applicationPermission);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool AddApplicationSettings(ApplicationSettings applicationSettings)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationSettings
                    .Add(applicationSettings);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool AddApplicationToken(ApplicationToken applicationToken)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationTokens
                    .Add(applicationToken);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool AddPermission(Permission permission)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .Permissions
                    .Add(permission);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool DeleteApplication(Application application)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .Applications
                    .Remove(application);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool DeleteApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationAddressConnections
                    .Remove(applicationAddressConnection);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool DeleteApplicationPermission(ApplicationPermission applicationPermission)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationPermissions
                    .Remove(applicationPermission);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool DeleteApplicationSettings(ApplicationSettings applicationSettings)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationSettings
                    .Remove(applicationSettings);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool DeleteApplicationToken(ApplicationToken applicationToken)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationTokens
                    .Remove(applicationToken);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool DeletePermission(Permission permission)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .Permissions
                    .Remove(permission);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public Application? GetApplication(int applicationId)
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext.Applications.Find(applicationId);
            }
        }

        public Application? GetApplication(string applicationName)
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext.Applications.Where(x => x.Name == applicationName).FirstOrDefault();
            }
        }

        public ApplicationAddressConnection? GetApplicationAddressConnection(int applicationId, int addressId, ApplicationConnectionTypes applicationConnectionTypes)
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext
                    .ApplicationAddressConnections
                    .Where(x => x.ApplicationId == applicationId &&
                            x.AddressId == addressId &&
                            x.ApplicationConnectionType == applicationConnectionTypes)
                    .FirstOrDefault();
            }
        }

        public List<ApplicationAddressConnection>? GetApplicationAddressConnectionsByAddress(int addressId)
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext
                    .ApplicationAddressConnections
                    .Where(x => x.AddressId == addressId)
                    .ToList();
            }
        }

        public List<ApplicationAddressConnection>? GetApplicationAddressConnectionsByApplication(int applicationId)
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext
                .ApplicationAddressConnections
                    .Where(x => x.ApplicationId == applicationId)
                    .ToList();
            }
        }

        public List<ApplicationPermission> GetApplicationPermission(int applicationId)
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext
                    .ApplicationPermissions
                    .Where(x => x.ApplicationId == applicationId)
                    .ToList();
            }
        }

        public ApplicationSettings? GetApplicationSettings(int applicationId)
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext
                    .ApplicationSettings
                    .Where(x => x.ApplicationId == applicationId)
                    .FirstOrDefault();
            }
        }

        public ApplicationToken? GetApplicationToken(int applicationId, string hashedSecret)
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext
                    .ApplicationTokens
                    .Where(x => x.ApplicationId == applicationId && x.HashedSecret == hashedSecret)
                    .FirstOrDefault();
            }
        }

        public ApplicationToken? GetApplicationToken(int applicationId, long id)
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext.ApplicationTokens.Find(id);
            }
        }

        public List<ApplicationToken> GetApplicationTokens(int applicationId)
        {
            throw new NotImplementedException();
        }

        public Permission? GetPermission(long permissionId)
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext.Permissions.Find(permissionId);
            }
        }

        public List<Permission> GetPermissions()
        {
            lock (appStateStoreDbContext)
            {
                return appStateStoreDbContext.Permissions.ToList();
            }
        }

        public bool UpdateApplication(Application application)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .Applications
                    .Update(application);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool UpdateApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection)
        {
            throw new NotImplementedException();
        }

        public bool UpdateApplicationPermission(ApplicationPermission applicationPermission)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationPermissions
                    .Update(applicationPermission);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool UpdateApplicationSettings(ApplicationSettings applicationSettings)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationSettings
                    .Update(applicationSettings);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool UpdateApplicationToken(ApplicationToken applicationToken)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .ApplicationTokens
                    .Update(applicationToken);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }

        public bool UpdatePermission(Permission permission)
        {
            lock (appStateStoreDbContext)
            {
                appStateStoreDbContext
                    .Permissions
                    .Update(permission);

                appStateStoreDbContext.SaveChanges();

                return true;
            }
        }
    }
}
