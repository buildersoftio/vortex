using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Dtos.Clients;
using Vortex.Core.Models.Entities.Clients.Applications;

namespace Vortex.Core.Abstractions.Services
{
    public interface IClientConnectionService
    {
        (List<ClientConnectionDto>? clientConnections, string message) GetClientConnectionsByApplicationName(string applicationName);
        (List<ClientConnectionDto>? clientConnections, string message) GetClientConnectionsByAddressName(string addressName);
        (List<ClientConnectionDto>? clientConnections, string message) GetClientConnectionsByAddressAlias(string addressAlias);

        (bool status, string message) RegisterClientConnection(ClientConnectionRequest clientConnectionRequest, string createdBy);
        (bool status, string message) VerifyClientConnectionByAddressAlias(string applicationName, string addressAlias, ApplicationConnectionTypes applicationType);
        (bool status, string message) VerifyClientConnectionByAddressName(string applicationName, string addressName, ApplicationConnectionTypes applicationType);

        bool RegisterClientHostConnection(string applicationName, string addressName, ApplicationConnectionTypes applicationType, string clientHost, string connectedNode);
        bool UpdateClientConnectionState(string applicationName, string addressName, ApplicationConnectionTypes applicationType, string clientHost, bool isConnected);

        // This method should be only used for internal purposes. PLEASE, do not expose it via REST Endpoints.
        ClientConnection? GetClientConnection(string applicationName, string addressName, ApplicationConnectionTypes applicationType);
        ClientConnection GetClientConnection(Guid connectionId);

        List<ClientConnection>? GetConnectedClientConnections();

        bool UpdateClientConnection(ClientConnection clientConnection);
    }
}
