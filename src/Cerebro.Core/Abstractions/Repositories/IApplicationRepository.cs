using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Entities.Clients.Applications;

namespace Cerebro.Core.Repositories
{
    public interface IApplicationRepository
    {
        bool AddApplication(Application application);
        bool DeleteApplication(Application application);
        bool UpdateApplication(Application application);
        Application? GetApplication(int applicationId);
        Application? GetApplication(string applicationName);


        bool AddApplicationSettings(ApplicationSettings applicationSettings);
        bool DeleteApplicationSettings(ApplicationSettings applicationSettings);
        bool UpdateApplicationSettings(ApplicationSettings applicationSettings);
        ApplicationSettings? GetApplicationSettings(int applicationId);


        bool AddApplicationToken(ApplicationToken applicationToken);
        bool DeleteApplicationToken(ApplicationToken applicationToken);
        bool UpdateApplicationToken(ApplicationToken applicationToken);
        ApplicationToken? GetApplicationToken(int applicationId, string hashedSecret);
        ApplicationToken? GetApplicationToken(int applicationId, long id);
        List<ApplicationToken> GetApplicationTokens(int applicationId);


        bool AddPermission(Permission permission);
        bool DeletePermission(Permission permission);
        bool UpdatePermission(Permission permission);
        Permission? GetPermission(long permissionId);
        List<Permission> GetPermissions();

        bool AddApplicationPermission(ApplicationPermission applicationPermission);
        bool DeleteApplicationPermission(ApplicationPermission applicationPermission);
        bool UpdateApplicationPermission(ApplicationPermission applicationPermission);
        List<ApplicationPermission> GetApplicationPermission(int applicationId);

        bool AddApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection);
        bool DeleteApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection);
        bool UpdateApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection);

        ApplicationAddressConnection? GetApplicationAddressConnection(int applicationId, int addressId, ApplicationConnectionTypes applicationConnectionTypes);
        List<ApplicationAddressConnection>? GetApplicationAddressConnectionsByApplication(int applicationId);
        List<ApplicationAddressConnection>? GetApplicationAddressConnectionsByAddress(int addressId);

    }
}
