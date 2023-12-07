using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Entities.Clients.Applications;
using Vortex.Core.Models.Routing.Integrations;

namespace Vortex.Core.Abstractions.Services.Routing
{
    public interface IClientCommunicationService
    {
        ClientConnectionResponse EstablishConnection(ClientConnectionRequest request, bool notifyOtherNodes = true);
        ClientConnectionResponse CloseConnection(ClientDisconnectionRequest request, bool notifyOtherNodes = true);
        ClientConnectionResponse HeartbeatConnection(Guid clientId, string clientHost, string applicationName, string address, TokenDetails tokenDetails, bool notifyOtherNodes = true);

        ClientConnection? GetClientConnection(string applicationName, string addressName, ApplicationConnectionTypes applicationType);
    }
}
