using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Dtos.Clients;

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
    }
}
