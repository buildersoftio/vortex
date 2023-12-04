using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Entities.Clients.Applications;

namespace Vortex.Core.Repositories
{
    public interface IApplicationRepository
    {
        bool AddApplication(Application application);
        bool DeleteApplication(Application application);
        bool UpdateApplication(Application application);
        Application? GetApplication(int applicationId);
        Application? GetApplication(string applicationName);
        List<Application> GetApplications(bool sendSoftDeleted = true);
        List<Application> GetActiveApplications();


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

        bool AddApplicationAddressConnection(ClientConnection applicationAddressConnection);
        bool DeleteApplicationAddressConnection(ClientConnection applicationAddressConnection);
        bool UpdateApplicationAddressConnection(ClientConnection applicationAddressConnection);

        ClientConnection? GetClientConnection(int applicationId, int addressId, ApplicationConnectionTypes applicationConnectionTypes);
        List<ClientConnection>? GetClientConnectionsByApplication(int applicationId);
        List<ClientConnection>? GetClientConnectionsByAddress(int addressId);

        List<ClientConnection>? GetConnectedClientConnections();

    }
}
