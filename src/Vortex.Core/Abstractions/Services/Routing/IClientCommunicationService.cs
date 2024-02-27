using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Data;
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

        (bool success, int? partitionKey, string message) AcceptMessage(Guid clientId, Span<PartitionMessage> partitionMessages);

        bool DeleteClientProducerFromCache(Guid clientId);

        InternalConsumeMessageResponse ConsumeNextMessage(Guid clientId, int addressId, int partitionId, long acknowledgedEntry);

        bool ValidateApplicationToken(Guid clientId, string appKey, string appSecret);
    }
}
