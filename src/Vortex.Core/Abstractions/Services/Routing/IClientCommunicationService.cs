using Vortex.Core.Models.Routing.Integrations;

namespace Vortex.Core.Abstractions.Services.Routing
{
    public interface IClientCommunicationService
    {
        ClientConnectionResponse EstablishConnection(ClientConnectionRequest request);
        ClientConnectionResponse CloseConnection(ClientDisconnectionRequest request);
    }
}
