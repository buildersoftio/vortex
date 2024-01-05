using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Models.Common.Addresses;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Entities.Clients.Applications;
using Vortex.Core.Utilities.Consts;
using Vortex.Core.Utilities.Json;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NodeExchange;
using Vortex.Core.Abstractions.Services.Routing;
using Vortex.Core.Abstractions.Services.Data;
using Vortex.Core.Abstractions.Services.Orchestrations;
using Vortex.Core.Models.Containers;
using Vortex.Core.Models.Data;

namespace Vortex.Cluster.Infrastructure.Servers.gRPC
{
    public class NodeExchangeServer : NodeExchangeService.NodeExchangeServiceBase, INodeExchangeServer
    {
        private readonly ILogger<NodeExchangeServer> _logger;

        private readonly int _port;
        private readonly int _portSSL;
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly Server _server;
        private readonly IAddressService _addressService;
        private readonly IApplicationService _applicationService;
        private readonly IClientCommunicationService _clientCommunicationService;

        private readonly IDataDistributionService _dataDistributionService;
        private readonly IServerCoreStateManager _serverCoreStateManager;

        public NodeExchangeServer(ILogger<NodeExchangeServer> logger,
            NodeConfiguration nodeConfiguration,
            IAddressService addressService,
            IApplicationService applicationService,
            IClientCommunicationService clientCommunicationService, IDataDistributionService dataDistributionService, IServerCoreStateManager serverCoreStateManager)
        {
            _logger = logger;
            _nodeConfiguration = nodeConfiguration;
            _addressService = addressService;
            _applicationService = applicationService;
            _clientCommunicationService = clientCommunicationService;

            // for data distribution
            _dataDistributionService = dataDistributionService;
            _serverCoreStateManager = serverCoreStateManager;


            if (Environment.GetEnvironmentVariable(EnvironmentConstants.VortexClusterConnectionPort) != null)
                _port = Convert.ToInt32(Environment.GetEnvironmentVariable(EnvironmentConstants.VortexClusterConnectionPort));
            else
                _port = -1;

            if (Environment.GetEnvironmentVariable(EnvironmentConstants.VortexClusterConnectionSSLPort) != null)
                _portSSL = Convert.ToInt32(Environment.GetEnvironmentVariable(EnvironmentConstants.VortexClusterConnectionSSLPort));
            else
                _portSSL = -1;

            string? hostName = Environment.GetEnvironmentVariable(EnvironmentConstants.VortexClusterHost);
            hostName ??= "localhost";

            _server = new Server()
            {
                Services = { NodeExchangeService.BindService(this) },
                Ports = { new ServerPort(hostName, _port, ServerCredentials.Insecure) }
            };
        }

        public Task ShutdownAsync()
        {
            if (_port != -1 || _portSSL != -1)
            {
                _logger.LogInformation($"Cluster node '{_nodeConfiguration.NodeId}' is shutdown");
                return _server.ShutdownAsync();
            }

            return Task.CompletedTask;
        }

        public void Start()
        {
            if (_port != -1 || _portSSL != -1)
                _server.Start();

            if (_port != -1)
                _logger.LogInformation("Cluster listening on port " + _port + " for node " + _nodeConfiguration.NodeId);

            if (_portSSL != -1)
                _logger.LogInformation("Cluster listening on port " + _port + " SSL for node " + _nodeConfiguration.NodeId);
        }

        public override Task<NodeRegistrationResponse> RegisterNode(NodeInfo request, ServerCallContext context)
        {
            _logger.LogInformation($"Node {request.NodeId} connection requested");
            return Task.FromResult(new NodeRegistrationResponse() { Message = "Node connected", Success = true });
        }

        public override Task<HeartbeatResponse> SendHeartbeat(Heartbeat request, ServerCallContext context)
        {
            _logger.LogInformation($"Heartbeat requested by node {request.NodeId} with ip {context.Peer}");
            return Task.FromResult(new HeartbeatResponse() { Success = true, Message = "Heartbeat received" });
        }


        #region Address Sync Region

        public override Task<AddressCreationResponse> RequestAddressCreation(AddressCreationRequest request, ServerCallContext context)
        {
            var addressCreationRequest = request.Address.JsonToObject<Core.Models.Dtos.Addresses.AddressCreationRequest>();

            _logger.LogInformation($"Address [{addressCreationRequest.Name}] creation requested by neighbor node {context.Peer}");

            (bool result, string message) = _addressService
                .CreateAddress(addressCreationRequest, request.CreatedBy, requestedByOtherNode: true);

            return Task.FromResult(new AddressCreationResponse() { Success = result });
        }

        public override Task<AddressResponse> RequestAddressPartitionChange(AddressPartitionChangeRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Address [{request.Alias}] partition change requested by neighbor node {context.Peer}");

            (bool result, string message) = _addressService.EditAddressPartitionSettings(request.Alias,
                new AddressPartitionSettings() { PartitionNumber = request.PartitionNumber }, request.UpdatedBy, requestedByOtherNode: true);

            return Task.FromResult(new AddressResponse() { Message = message, Success = result });
        }

        public override Task<AddressResponse> RequestAddressReplicationSettingsChange(AddressReplicationSettingsChangeRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Address [{request.Alias}] replication change requested by neighbor node {context.Peer}");

            (bool result, string message) = _addressService
                .EditAddressReplicationSettings(request.Alias, request.ReplicationSettingsJson.JsonToObject<AddressReplicationSettings>(), request.UpdatedBy, requestedByOtherNode: true);

            return Task.FromResult(new AddressResponse() { Message = message, Success = result });
        }


        public override Task<AddressResponse> RequestAddressRetentionSettingsChange(AddressRetentionSettingsChangeRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Address [{request.Alias}] retention change requested by neighbor node {context.Peer}");

            (bool result, string message) = _addressService
                .EditAddressRetentionSettings(request.Alias, request.RetentionSettingsJson.JsonToObject<AddressRetentionSettings>(), request.UpdatedBy, requestedByOtherNode: true);

            return Task.FromResult(new AddressResponse() { Message = message, Success = result });
        }

        public override Task<AddressResponse> RequestAddressSchemaSettingsChange(AddressSchemaSettingsChangeRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Address [{request.Alias}] schema change requested by neighbor node {context.Peer}");

            (bool result, string message) = _addressService
                .EditAddressSchemaSettings(request.Alias, request.SchemaSettingsJson.JsonToObject<AddressSchemaSettings>(), request.UpdatedBy, requestedByOtherNode: true);

            return Task.FromResult(new AddressResponse() { Message = message, Success = result });
        }

        public override Task<AddressResponse> RequestAddressStorageSettingsChange(AddressStorageSettingsChangeRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Address [{request.Alias}] storage change requested by neighbor node {context.Peer}");
            (bool result, string message) = _addressService
                .EditAddressStorageSettings(request.Alias, request.StorageSettingsJson.JsonToObject<AddressStorageSettings>(), request.UpdatedBy, requestedByOtherNode: true);

            return Task.FromResult(new AddressResponse() { Message = message, Success = result });
        }

        public override Task<AddressResponse> RequestAddressDeletion(AddressDeletionRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Address [{request.Alias}] deletion requested by neighbor node {context.Peer}");

            (bool result, string message) = _addressService
                .DeleteAddress(request.Alias, requestedByOtherNode: true);

            return Task.FromResult(new AddressResponse() { Message = message, Success = result });
        }

        #endregion

        #region Application Sync Region

        public override Task<ApplicationResponse> RequestApplicationCreation(ApplicationCreationRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Application [{request.Name}] creation requested by neighbor node {context.Peer}");

            (bool result, string message) = _applicationService.CreateApplication(new Core.Models.Dtos.Applications.ApplicationDto()
            {
                Name = request.Name,
                Description = request.Description,
                Settings = request.SettingsJson.JsonToObject<ApplicationSettings>()
            }, request.CreatedBy, requestedByOtherNode: true);

            return Task.FromResult(new ApplicationResponse() { Message = message, Success = result });
        }

        public override Task<ApplicationResponse> RequestApplicationDeletion(ApplicationDeletionRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Application [{request.ApplicationName}] deletion requested by neighbor node {context.Peer}");
            bool result;
            string message;

            if (request.IsHardDelete == true)
            {
                (result, message) = _applicationService.HardDeleteApplication(request.ApplicationName, requestedByOtherNode: true);
            }
            else
            {
                (result, message) = _applicationService.SoftDeleteApplication(request.ApplicationName, request.UpdatedBy, requestedByOtherNode: true);
            }

            return Task.FromResult(new ApplicationResponse() { Message = message, Success = result });
        }

        public override Task<ApplicationResponse> RequestApplicationUpdate(ApplicationUpdateRequest request, ServerCallContext context)
        {
            bool result;
            string message;

            if (request.SettingsJson == "DESCRIPTION_ONLY")
            {
                _logger.LogInformation($"Application [{request.Name}] description update requested by neighbor node {context.Peer}");
                (result, message) = _applicationService.EditApplicationDescription(request.Name, request.Description, request.UpdatedBy, requestedByOtherNode: true);
            }
            else
            {
                _logger.LogInformation($"Application [{request.Name}] settings update requested by neighbor node {context.Peer}");
                (result, message) = _applicationService.EditApplicationSettings(request.Name, request.SettingsJson.JsonToObject<ApplicationSettings>(), request.UpdatedBy, requestedByOtherNode: true);
            }

            return Task.FromResult(new ApplicationResponse() { Message = message, Success = result });
        }

        public override Task<ApplicationResponse> RequestApplicationPermissionChange(ChangeApplicationPermissionRequest request, ServerCallContext context)
        {
            bool result;
            string message;

            if (request.PermissionType == "READ_ADDRESSES")
            {
                _logger.LogInformation($"Application [{request.ApplicationName}] permission update of [READ_ADDRESSES] requested by neighbor node {context.Peer}");
                (result, message) = _applicationService.EditReadAddressApplicationPermission(request.ApplicationName, request.Value, request.UpdatedBy, requestedByOtherNode: true);

                return Task.FromResult(new ApplicationResponse() { Message = message, Success = result });
            }

            if (request.PermissionType == "WRITE_ADDRESSES")
            {
                _logger.LogInformation($"Application [{request.ApplicationName}] permission update of [WRITE_ADDRESSES] requested by neighbor node {context.Peer}");

                (result, message) = _applicationService.EditWriteAddressApplicationPermission(request.ApplicationName, request.Value, request.UpdatedBy, requestedByOtherNode: true);

                return Task.FromResult(new ApplicationResponse() { Message = message, Success = result });
            }

            if (request.PermissionType == "CREATE_ADDRESSES")
            {
                _logger.LogInformation($"Application [{request.ApplicationName}] permission update of [CREATE_ADDRESSES] requested by neighbor node {context.Peer}");
                (result, message) = _applicationService.EditCreateAddressApplicationPermission(request.ApplicationName, Convert.ToBoolean(request.Value), request.UpdatedBy, requestedByOtherNode: true);

                return Task.FromResult(new ApplicationResponse() { Message = message, Success = result });
            }

            return Task.FromResult(new ApplicationResponse() { Message = "FATAL_ERROR", Success = false });
        }

        public override Task<ApplicationResponse> RequestApplicationStatusChange(ApplicationActivationRequest request, ServerCallContext context)
        {
            bool result;
            string message;

            if (request.IsActive == true)
            {
                _logger.LogInformation($"Application [{request.Name}] activation update requested by neighbor node {context.Peer}");

                (result, message) = _applicationService.ActivateApplication(request.Name, request.UpdatedBy, requestedByOtherNode: true);
            }
            else
            {
                _logger.LogInformation($"Application [{request.Name}] deactivation update requested by neighbor node {context.Peer}");

                (result, message) = _applicationService.DeactivateApplication(request.Name, request.UpdatedBy, requestedByOtherNode: true);
            }

            return Task.FromResult(new ApplicationResponse() { Message = message, Success = result });
        }

        public override Task<ApplicationResponse> RequestApplicationTokenCreation(AddApplicationTokenRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Application [{request.ApplicationName}] token creation requested by neighbor node {context.Peer}");

            // we are using this internal method just for token sync between nodes in the cluster
            (bool result, string message) = _applicationService.CreateInternalApplicationToken(request.ApplicationName,
                request.ApplicationTokenEntityJson.JsonToObject<ApplicationToken>());

            return Task.FromResult(new ApplicationResponse() { Message = message, Success = result });
        }

        public override Task<ApplicationResponse> RequestApplicationTokenRevocation(RevokeApplicationTokenRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Application [{request.ApplicationName}] token revocation for [{request.ApiKey}] requested by neighbor node {context.Peer}");

            // we are using this internal method just for token sync between nodes in the cluster
            (bool result, string message) = _applicationService.RevokeApplicationToken(request.ApplicationName, Guid.Parse(request.ApiKey), request.UpdatedBy, requestedByOtherNode: true);

            return Task.FromResult(new ApplicationResponse() { Message = message, Success = result });
        }

        #endregion


        #region Client connection

        public override Task<ClientConnection_Response> RequestClientConnectionRegistration(ClientConnection_Registration request, ServerCallContext context)
        {
            _logger.LogInformation($"Application [{request.Application}] client connection registration requested by neighbor node [{context.Peer}]");

            var response = _clientCommunicationService.EstablishConnection(new Core.Models.Routing.Integrations.ClientConnectionRequest()
            {
                Address = request.Address,
                Application = request.Application,
                AppKey = request.AppKey,
                AppSecret = request.AppToken,
                ConnectedNode = request.ConnectedNode,
                ClientHost = request.ClientHost,
                ApplicationType = Enum.Parse<ApplicationConnectionTypes>(request.ConnectionType),

                ProductionInstanceType = Enum.Parse<ProductionInstanceTypes>(request.ProductionInstanceType),
                ReadInitialPosition = Enum.Parse<ReadInitialPositions>(request.ReadInitialPosition),
                SubscriptionMode = Enum.Parse<SubscriptionModes>(request.SubscriptionMode),
                SubscriptionType = Enum.Parse<SubscriptionTypes>(request.SubscriptionType)

            }, notifyOtherNodes: false);

            if (response.Status == Core.Models.Routing.Common.ConnectionStatuses.Connected)
                return Task.FromResult(new ClientConnection_Response() { Message = response.Message, Success = true });

            return Task.FromResult(new ClientConnection_Response() { Message = response.Message, Success = false });
        }

        public override Task<ClientConnection_Response> RequestClientConnectionHeartbeat(ClientConnection_Heartbeat request, ServerCallContext context)
        {
            var clientConnection = _clientCommunicationService.GetClientConnection(request.Application, request.Address, Enum.Parse<ApplicationConnectionTypes>(request.ConnectionType));
            if (clientConnection == null)
                return Task.FromResult(new ClientConnection_Response() { Message = $"Client connection doesnot exists for application [{request.Application}] and address [{request.Address}]", Success = false });

            var response = _clientCommunicationService.HeartbeatConnection(clientConnection.Id, request.ClientHost, request.Application, request.Address, new TokenDetails()
            {
                AppKey = request.AppKey,
                AppSecret = request.AppToken
            }, notifyOtherNodes: false);

            if (response.Status == Core.Models.Routing.Common.ConnectionStatuses.Connected)
                return Task.FromResult(new ClientConnection_Response() { Message = response.Message, Success = true });

            return Task.FromResult(new ClientConnection_Response() { Message = response.Message, Success = false });
        }

        public override Task<ClientConnection_Response> RequestClientConnectionDisconnect(ClientConnection_Close request, ServerCallContext context)
        {
            var clientConnection = _clientCommunicationService.GetClientConnection(request.Application, request.Address, Enum.Parse<ApplicationConnectionTypes>(request.ConnectionType));
            if (clientConnection == null)
                return Task.FromResult(new ClientConnection_Response() { Message = $"Client connection doesnot exists for application [{request.Application}] and address [{request.Address}]", Success = false });

            var response = _clientCommunicationService.CloseConnection(new Core.Models.Routing.Integrations.ClientDisconnectionRequest()
            {
                ApplicationType = Enum.Parse<ApplicationConnectionTypes>(request.ConnectionType),
                Address = request.Address,
                AppKey = request.AppKey,
                AppSecret = request.AppToken,
                Application = request.Application,
                ClientHost = request.ClientHost,
                ClientId = request.ClientId
            }, notifyOtherNodes: false);

            if (response.Status == Core.Models.Routing.Common.ConnectionStatuses.Disconnected)
                return Task.FromResult(new ClientConnection_Response() { Message = response.Message, Success = true });

            return Task.FromResult(new ClientConnection_Response() { Message = response.Message, Success = false });
        }

        #endregion

        public override Task<DataDistribution_Response> DistributeData(DataDistributionMessage request, ServerCallContext context)
        {
            // check if the address is loaded in memory
            // loading address in memory should be the last thing ever :p it's me saying it!
            if (_serverCoreStateManager.IsAddressPartitionsLoaded(request.AddressAlias) != true)
                _serverCoreStateManager.LoadAddressPartitionsInMemory(request.AddressAlias);

            AddressContainer? address = _serverCoreStateManager.GetAddressContainer(request.AddressAlias);

            if (address == null)
                return Task.FromResult(new DataDistribution_Response() { Success = false });

            (bool success, int? partition, string message) = _dataDistributionService.Distribute(address.AddressName!, new PartitionMessage()
            {
                MessageId = request.MessageId.ToByteArray(),
                MessagePayload = request.MessagePayload.ToByteArray(),
                MessageHeaders = new Dictionary<string, string>(request.MessageHeaders),
                PartitionIndex = request.PartitionIndex,
                HostApplication = context.Peer,
                SourceApplication = request.SourceApplication,
                SentDate = request.SentDate.ToDateTime(),
                StoredDate = DateTime.Now
            });

            return Task.FromResult(new DataDistribution_Response() { Success = success });
        }
    }
}

