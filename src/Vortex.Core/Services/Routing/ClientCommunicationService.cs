using Microsoft.Extensions.Logging;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Abstractions.Services.Orchestrations;
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
        private readonly IServerCoreStateManager _serverCoreStateManager;

        public ClientCommunicationService(ILogger<ClientCommunicationService> logger,
            IApplicationService applicationService,
            IAddressService addressService,
            IClientConnectionService clientConnectionService,
            IServerCoreStateManager serverCoreStateManager)
        {
            _logger = logger;
            _applicationService = applicationService;
            _addressService = addressService;
            _clientConnectionService = clientConnectionService;

            _serverCoreStateManager = serverCoreStateManager;
        }

        public ClientConnectionResponse EstablishConnection(ClientConnectionRequest request)
        {
            // Verify application Token, by checking the settings of Application.
            (var applicationDto, var message) = _applicationService.GetApplication(request.Application);

            // TODO: Add settings for the node. If new Applications can be created on the spot from the vortex clients.
            // For now, we are igonring this part.

            _logger.LogInformation($"Application [{request.Application}] client requested new connection from [{request.ClientHost}]");

            if (applicationDto == null)
            {
                _logger.LogInformation($"Application [{request.Application}] does not exists or is disabled, connection is closing");
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

            // get default values from application settings if are not provided from the client...
            request.ProductionInstanceType ??= applicationDto.Settings.DefaultProductionInstanceType;
            request.SubscriptionType ??= applicationDto.Settings.DefaultSubscriptionType;
            request.SubscriptionMode ??= applicationDto.Settings.DefaultSubscriptionMode;
            request.ReadInitialPosition ??= applicationDto.Settings.DefaultReadInitialPosition;

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
            _clientConnectionService.UpdateClientConnectionState(request.Application, request.Address, request.ApplicationType, request.ClientHost, isConnected: true);

            var clientConnection = _clientConnectionService.GetClientConnection(request.Application, request.Address, request.ApplicationType);
            _logger.LogInformation($"Application [{request.Application}] is connected to address [{request.Address}]");

            
            // loading address in memory
            (var address, var _) = _addressService.GetAddressByName(request.Address);
            if (address != null)
            {
                _serverCoreStateManager.LoadAddressPartitionsInMemory(address.Alias);
            }


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
            (var applicationDto, var message) = _applicationService.GetApplication(request.Application);
            _logger.LogInformation($"Application [{request.Application}] client requested to disconnect from id [{request.ClientId}] with host [{request.ClientHost}]");

            if (applicationDto == null)
            {
                _logger.LogInformation($"Application [{request.Application}] does not exists or is disabled, connection is closing");
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

            var clientConnectionDetails = _clientConnectionService.GetClientConnection(request.Application, request.Address, request.ApplicationType);
            if (clientConnectionDetails == null)
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.Error, Message = "Something went wrong! ClientConnection does not exists" };

            if (clientConnectionDetails.Id != Guid.Parse(request.ClientId))
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.FatalError, Message = $"Provided ClientId [{request.ClientId}] is not registered in ClientConnections for Application [{request.Application}]" };


            _clientConnectionService.UpdateClientConnectionState(request.Application, request.Address, request.ApplicationType, request.ClientHost, isConnected: false);

            _logger.LogInformation($"Application [{request.Application}] connection from [{request.ClientHost}] is disconnected from address [{request.Address}]");

            // trying to unload address from memory.
            // TODO:.. we are here...

            return new ClientConnectionResponse()
            {
                ApplicationId = request.ApplicationId,
                Status = Models.Routing.Common.ConnectionStatuses.Disconnected,
                Message = "Application connection has been released"
            };
        }
    }
}
