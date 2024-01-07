using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Abstractions.Services.Data;
using Vortex.Core.Abstractions.Services.Orchestrations;
using Vortex.Core.Abstractions.Services.Routing;
using Vortex.Core.Models.BackgroundRequests;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Data;
using Vortex.Core.Models.Entities.Clients.Applications;
using Vortex.Core.Models.Routing.Integrations;
using Vortex.Core.Utilities.Consts;

namespace Vortex.Core.Services.Routing
{
    public class ClientCommunicationService : IClientCommunicationService
    {
        private readonly ILogger<ClientCommunicationService> _logger;
        private readonly IApplicationService _applicationService;
        private readonly IAddressService _addressService;
        private readonly IClientConnectionService _clientConnectionService;
        private readonly IServerCoreStateManager _serverCoreStateManager;
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly IBackgroundQueueService<ClientConnectionBackgroundRequest> _clusterClientConnectionService;
        private readonly IDataDistributionService _dataDistributionService;

        private readonly ConcurrentDictionary<Guid, int> _clientProducerAddressCache;

        public ClientCommunicationService(ILogger<ClientCommunicationService> logger,
            IApplicationService applicationService,
            IAddressService addressService,
            IClientConnectionService clientConnectionService,
            IServerCoreStateManager serverCoreStateManager,
            NodeConfiguration nodeConfiguration,
            IBackgroundQueueService<ClientConnectionBackgroundRequest> clusterClientConnectionService,
            IDataDistributionService dataDistributionService)
        {
            _logger = logger;
            _applicationService = applicationService;
            _addressService = addressService;
            _clientConnectionService = clientConnectionService;

            _serverCoreStateManager = serverCoreStateManager;

            _nodeConfiguration = nodeConfiguration;
            _clusterClientConnectionService = clusterClientConnectionService;
            _dataDistributionService = dataDistributionService;

            _clientProducerAddressCache = new ConcurrentDictionary<Guid, int>();
        }

        public ClientConnectionResponse EstablishConnection(ClientConnectionRequest request, bool notifyOtherNodes = true)
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

            if (applicationDto.Settings.IsAuthorizationEnabled == true && notifyOtherNodes == true)
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
            request.ConsumptionSettings ??= applicationDto.Settings.ConsumptionSettings;


            (bool isClientConnectionRegistered, string error) = _clientConnectionService.RegisterClientConnection(new Models.Dtos.Clients.ClientConnectionRequest()
            {
                Address = request.Address,
                ApplicationConnectionType = request.ApplicationType,
                ApplicationName = applicationDto.Name,
                ProductionInstanceType = request.ProductionInstanceType,
                ConsumptionSettings = request.ConsumptionSettings

            }, request.Application);

            if (isClientConnectionRegistered != true)
            {
                _logger.LogInformation($"Application [{request.Application}] is forbidden to access address [{request.Address}] resources. Error details: {error}");
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.Forbidden, Message = error };
            }

            // Update or create a client connection - add client IP Address...
            _clientConnectionService.RegisterClientHostConnection(request.Application, request.Address, request.ApplicationType, request.ClientHost, request.ConnectedNode);
            _clientConnectionService.UpdateClientConnectionState(request.Application, request.Address, request.ApplicationType, request.ClientHost, isConnected: true);

            var clientConnection = _clientConnectionService.GetClientConnection(request.Application, request.Address, request.ApplicationType);
            _logger.LogInformation($"Application [{request.Application}] is connected to address [{request.Address}]");


            // loading address in memory
            (var address, var _) = _addressService.GetAddressByName(request.Address);

            if (address == null)
            {
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.FatalError, Message = $"Address [{request.Address}] doesnot exists" };
            }

            // here we check the status of the address, if the address is not ready, we try for 3 seconds.
            int k = 0;
            while (address.Status != Models.Common.Addresses.AddressStatuses.Ready)
            {
                if (k == _nodeConfiguration.CheckRetryCount)
                {
                    _logger.LogError($"Application client from [{request.ClientHost}] cannot connect, address [{address.Name}] partitions failed to create.");
                    return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.FatalError, Message = $"Address [{address.Name}] partitions are not created. Application client cannot connect. To increase the number of retries change Environment variable [{EnvironmentConstants.BackgroundCheckRetryCount}]" };
                }

                Thread.Sleep(1000);
                k++;
            }

            // inform other nodes for Application Client connection
            if (address!.Settings.Scope == Models.Common.Addresses.AddressScope.ClusterScope && notifyOtherNodes == true)
            {
                _clusterClientConnectionService.EnqueueRequest(new ClientConnectionBackgroundRequest()
                {
                    RequestState = ClientConnectionRequestState.EstablishConnection,

                    Application = request.Application,
                    Address = request.Address,
                    ApplicationType = request.ApplicationType,
                    ClientHost = request.ClientHost,

                    ConnectedNode = _nodeConfiguration.NodeId,
                    Credentials = new TokenDetails() { AppKey = request.AppKey, AppSecret = request.AppSecret },

                    ProductionInstanceType = request.ProductionInstanceType,
                    ConsumptionSettings = request.ConsumptionSettings

                });
            }

            // loading address in memory should be the last thing ever :p it's me saying it!
            _serverCoreStateManager.LoadAddressPartitionsInMemory(address.Alias);

            return new ClientConnectionResponse()
            {
                ClientId = clientConnection!.Id,
                Status = Models.Routing.Common.ConnectionStatuses.Connected,
                Message = $"[{request.Application}] is connected to [{request.Address}]"
            };
        }

        public ClientConnectionResponse CloseConnection(ClientDisconnectionRequest request, bool notifyOtherNodes = true)
        {
            (var applicationDto, var message) = _applicationService.GetApplication(request.Application);
            _logger.LogInformation($"Application [{request.Application}] client requested to disconnect from id [{request.ClientId}] with host [{request.ClientHost}]");

            if (applicationDto == null)
            {
                _logger.LogInformation($"Application [{request.Application}] does not exists or is disabled, connection is closing");
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.ApplicationNotFound, Message = message };
            }

            if (applicationDto.Settings.IsAuthorizationEnabled == true && notifyOtherNodes == true)
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

            // added && notifyOtherNodes == true; reason: the connection id-s are different for each node when the client-connection is registered.
            // we check this condition only when the request is coming to this node
            if (clientConnectionDetails.Id != Guid.Parse(request.ClientId) && notifyOtherNodes == true)
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.FatalError, Message = $"Provided ClientId [{request.ClientId}] is not registered in ClientConnections for Application [{request.Application}]" };


            _clientConnectionService.UpdateClientConnectionState(request.Application, request.Address, request.ApplicationType, request.ClientHost, isConnected: false);

            _logger.LogInformation($"Application [{request.Application}] connection from [{request.ClientHost}] is disconnected from address [{request.Address}]");


            // get address by id 
            (var address, var _) = _addressService.GetAddressById(clientConnectionDetails.AddressId);
            if (address == null)
            {
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.FatalError, Message = $"Address with id [{clientConnectionDetails.AddressId}] doesnot exists" };
            }

            // inform other nodes for Application Client connection
            if (address!.Settings.Scope == Models.Common.Addresses.AddressScope.ClusterScope && notifyOtherNodes == true)
            {
                _clusterClientConnectionService.EnqueueRequest(new ClientConnectionBackgroundRequest()
                {
                    RequestState = ClientConnectionRequestState.ClientDisconnection,

                    Application = request.Application,
                    Address = request.Address,
                    ApplicationType = request.ApplicationType,
                    ClientId = request.ClientId,
                    ClientHost = request.ClientHost,
                    Credentials = new TokenDetails() { AppKey = request.AppKey, AppSecret = request.AppSecret },
                    ConnectedNode = _nodeConfiguration.NodeId
                });
            }

            // trying to unload address from memory.
            // TODO:.. we are here...

            return new ClientConnectionResponse()
            {
                Status = Models.Routing.Common.ConnectionStatuses.Disconnected,
                Message = "Application connection has been released"
            };
        }

        public ClientConnectionResponse HeartbeatConnection(Guid clientId, string clientHost, string applicationName, string addressName, TokenDetails tokenDetails, bool notifyOtherNodes = true)
        {
            (var applicationDto, var message) = _applicationService.GetApplication(applicationName);

            if (applicationDto == null)
            {
                _logger.LogInformation($"Application [{applicationName}] does not exists or is disabled, connection is closing");
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.ApplicationNotFound, Message = message };
            }

            if (applicationDto.Settings.IsAuthorizationEnabled == true && notifyOtherNodes == true)
            {
                var isAppKeyValid = Guid.TryParse(tokenDetails.AppKey, out Guid appKey);
                if (isAppKeyValid != true)
                    appKey = Guid.Empty;

                if (_applicationService.IsValidToken(applicationName, appKey, tokenDetails.AppSecret) != true)
                {
                    _logger.LogInformation($"Application [{applicationName}] is not authorized. Provided appKey or appSecret is invalid or has expired");
                    return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.NotAuthorized, Message = "Provided AppKey or AppSecret is invalid or has expired, please contact Admin to generate a new APP KEY and APP SECRET" };
                }
            }

            var clientConnection = _clientConnectionService.GetClientConnection(clientId);
            if (clientConnection == null)
            {
                _logger.LogError($"Application client send heartbeat request, client connection doesnot exists at id [{clientId}] for application [{applicationName}] linked with [{addressName}]");
                return new ClientConnectionResponse() { ClientId = clientId, Status = Models.Routing.Common.ConnectionStatuses.Error, Message = $"Client connection doesnot exists at id {clientId} for application {applicationName} linked with {addressName}" };
            }

            // find this connection in ConnectionHistory
            if (clientConnection.HostsHistory.ContainsKey(clientHost) != true)
            {
                return new ClientConnectionResponse() { ClientId = clientId, Status = Models.Routing.Common.ConnectionStatuses.Error, Message = $"Something went wrong! Host [{clientHost}] is not connected to Vortex Server." };
            }

            if (clientConnection.HostsHistory[clientHost].IsConnected != true)
                return new ClientConnectionResponse() { ClientId = clientId, Status = Models.Routing.Common.ConnectionStatuses.Error, Message = $"Something went wrong! Host [{clientHost}] is already disconnected from Vortex Server." };

            clientConnection.HostsHistory[clientHost].LastHeartbeatDate = DateTime.UtcNow;
            _clientConnectionService.UpdateClientConnection(clientConnection);


            // send heartbeat to other nodes
            // get address by id 
            (var address, var _) = _addressService.GetAddressById(clientConnection.AddressId);
            if (address == null)
            {
                return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.FatalError, Message = $"Address with id [{clientConnection.AddressId}] doesnot exists" };
            }

            // inform other nodes for Application Client heartbeat
            if (address!.Settings.Scope == Models.Common.Addresses.AddressScope.ClusterScope && notifyOtherNodes == true)
            {
                // Check if the server is running under the cluster.

                _clusterClientConnectionService.EnqueueRequest(new ClientConnectionBackgroundRequest()
                {
                    RequestState = ClientConnectionRequestState.HeartbeatConnection,

                    ConnectedNode = _nodeConfiguration.NodeId,
                    ClientHost = clientHost,
                    Credentials = tokenDetails,
                    Application = applicationName,
                    Address = addressName,
                    ApplicationType = clientConnection.ApplicationConnectionType,
                    ClientId = clientConnection.Id.ToString(),
                });
            }

            return new ClientConnectionResponse() { Status = Models.Routing.Common.ConnectionStatuses.Connected, ClientId = clientId, Message = "Heartbeat received" };
        }

        public ClientConnection? GetClientConnection(string applicationName, string addressName, ApplicationConnectionTypes applicationType)
        {
            return _clientConnectionService
                .GetClientConnection(applicationName, addressName, applicationType);
        }

        public (bool success, int? partitionKey, string message) AcceptMessage(Guid clientId, Span<PartitionMessage> partitionMessages)
        {
            int addressId = GetAddressFromCache(clientId);
            if (addressId != -1)
                return _dataDistributionService.Distribute(addressId, partitionMessages);

            return (success: false, partitionKey: -1, "The given address name doesnot exists");

        }

        private int GetAddressFromCache(Guid clientId)
        {
            if (_clientProducerAddressCache.ContainsKey(clientId))
                return _clientProducerAddressCache[clientId];

            var clientConnection = _clientConnectionService.GetClientConnection(clientId);
            if (clientConnection == null)
                return -1;

            _clientProducerAddressCache.TryAdd(clientId, clientConnection.AddressId);

            return clientConnection.AddressId;
        }

        public bool DeleteClientProducerFromCache(Guid clientId)
        {
            if (_clientProducerAddressCache.ContainsKey(clientId))
                return _clientProducerAddressCache.TryRemove(clientId, out _);

            return true;
        }
    }
}
