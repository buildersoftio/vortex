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
        List<Application> GetApplications();


        bool AddApplicationToken(ApplicationToken applicationToken);
        bool DeleteApplicationToken(ApplicationToken applicationToken);
        bool UpdateApplicationToken(ApplicationToken applicationToken);
        ApplicationToken? GetApplicationToken(int applicationId, string hashedSecret);
        ApplicationToken? GetApplicationToken(Guid key, string hashedSecret);
        ApplicationToken? GetApplicationToken(int applicationId, Guid id);
        List<ApplicationToken> GetApplicationTokens(int applicationId);


        bool AddApplicationPermission(ApplicationPermission applicationPermission);
        bool DeleteApplicationPermission(ApplicationPermission applicationPermission);
        bool UpdateApplicationPermission(ApplicationPermission applicationPermission);
        ApplicationPermission GetApplicationPermission(int applicationId);

        bool AddApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection);
        bool DeleteApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection);
        bool UpdateApplicationAddressConnection(ApplicationAddressConnection applicationAddressConnection);

        ApplicationAddressConnection? GetApplicationAddressConnection(int applicationId, int addressId, ApplicationConnectionTypes applicationConnectionTypes);
        List<ApplicationAddressConnection>? GetApplicationAddressConnectionsByApplication(int applicationId);
        List<ApplicationAddressConnection>? GetApplicationAddressConnectionsByAddress(int addressId);

    }
}
