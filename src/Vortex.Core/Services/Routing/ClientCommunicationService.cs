using Microsoft.Extensions.Logging;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Abstractions.Services.Routing;
using Vortex.Core.Models.Routing.Integrations;

namespace Vortex.Core.Services.Routing
{
    public class ClientCommunicationService : IClientCommunicationService
    {
        private readonly ILogger<ClientCommunicationService> _logger;
        private readonly IApplicationService _applicationService;
        private readonly IAddressService _addressService;
        private readonly IClientConnectionService _clientConnectionService;

        public ClientCommunicationService(ILogger<ClientCommunicationService> logger,
            IApplicationService applicationService,
            IAddressService addressService,
            IClientConnectionService clientConnectionService)
        {
            _logger = logger;
            _applicationService = applicationService;
            _addressService = addressService;
            _clientConnectionService = clientConnectionService;
        }

        public ClientConnectionResponse EstablishConnection(ClientConnectionRequest request)
        {
            // Verify application Token, by checking the settings of Application.
            (var applicationDto, var message) = _applicationService.GetApplication(request.Application);

            // TODO: Add settings for the node. If new Applications can be created on the spot from the vortex clients.
            // For now, we are igonring this part.

            _logger.LogInformation($"Application [{request.Application}] client requested new connection");

            if (applicationDto == null)
            {
                _logger.LogInformation($"Application [{request.Application}] doesnot exists or is disabled, connection is closing");
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.ApplicationNotFound, Message = message };
            }

            if (applicationDto.Settings.IsAuthorizationEnabled == true)
            {
                var isAppKeyValid = Guid.TryParse(request.AppKey, out Guid appKey);
                if (isAppKeyValid != true)
                    appKey = Guid.Empty;

                if (_applicationService.IsValidToken(request.Application, appKey, request.AppSecret) != true)
                {
                    _logger.LogInformation($"Application [{request.Application}] is not authorized. Provided appKey or appSecret is invalid or has expired");
                    return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.NotAuthorized, Message = "Provided AppKey or AppSecret is invalid or has expired, please contact Admin to generate a new APP KEY and APP SECRET" };
                }
            }

            (bool isClientConnectionRegistered, string error) = _clientConnectionService.RegisterClientConnection(new Models.Dtos.Clients.ClientConnectionRequest()
            {
                Address = request.Address,
                ApplicationConnectionType = request.ApplicationType,
                ApplicationName = applicationDto.Name,
                ProductionInstanceType = request.ProductionInstanceType,
                ReadInitialPosition = request.ReadInitialPosition,
                SubscriptionMode = request.SubscriptionMode,
                SubscriptionType = request.SubscriptionType
            }, request.Application);

            if (isClientConnectionRegistered != true)
            {
                _logger.LogInformation($"Application [{request.Application}] is forbidden to access address [{request.Address}] resources. Error details: {error}");
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.Forbidden, Message = error };
            }

            // Update or create a client connection - add client IP Address...
            _clientConnectionService.RegisterClientHostConnection(request.Application, request.Address, request.ApplicationType, request.ClientHost);
            _clientConnectionService.UpdateClientConnectionState(request.Application, request.Address, request.ApplicationType, isConnected: true);

            var clientConnection = _clientConnectionService.GetClientConnection(request.Application, request.Address, request.ApplicationType);
            _logger.LogInformation($"Application [{request.Application}] is connected to address [{request.Address}]");

            return new ClientConnectionResponse()
            {
                ApplicationId = applicationDto.Id,
                ClientId = clientConnection!.Id,
                Status = Models.Routing.Common.ConnectionStatuses.Connected,
                Message = $"[{request.Application}] is connected to [{request.Address}]"
            };
        }

        public ClientConnectionResponse CloseConnection(ClientDisconnectionRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
