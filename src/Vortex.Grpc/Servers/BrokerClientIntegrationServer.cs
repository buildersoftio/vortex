using ApplicationIntegration;
using Vortex.Core.Abstractions.Clients;
using Vortex.Core.Utilities.Consts;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Vortex.Core.Abstractions.Services.Routing;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Routing.Integrations;
using System.Reflection.PortableExecutable;
using Vortex.Core.Models.Configurations;

namespace Vortex.Grpc.Servers
{
    public class BrokerClientIntegrationServer : DataTransmissionService.DataTransmissionServiceBase, IClientIntegrationServer
    {
        private readonly ILogger<BrokerClientIntegrationServer> _logger;
        private readonly IClientCommunicationService _clientCommunicationService;
        private readonly NodeConfiguration _nodeConfiguration;

        private readonly Server _server;
        private readonly int _port;
        private readonly int _portSSL;

        public BrokerClientIntegrationServer(
            ILogger<BrokerClientIntegrationServer> logger,
            IClientCommunicationService clientCommunicationService,
            NodeConfiguration nodeConfiguration)
        {
            _logger = logger;
            _clientCommunicationService = clientCommunicationService;
            _nodeConfiguration = nodeConfiguration;

            if (Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerPort) != null)
                _port = Convert.ToInt32(Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerPort));
            else
                _port = -1;

            if (Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerConnectionSSLPort) != null)
                _portSSL = Convert.ToInt32(Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerConnectionSSLPort));
            else
                _portSSL = -1;

            string? hostName = Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerHost);
            hostName ??= "localhost";

            if (_port != -1)
            {
                _server = new Server()
                {
                    Services = { DataTransmissionService.BindService(this) },
                    Ports = { new ServerPort(hostName, _port, ServerCredentials.Insecure) }
                };
            }

            if (_portSSL != -1)
            {
                _server = new Server()
                {
                    Services = { DataTransmissionService.BindService(this) },
                    Ports = { new ServerPort(hostName, _portSSL, ServerCredentials.Insecure) }
                };
            }

            if (_portSSL != -1 && _port != -1)
            {
                _server = new Server()
                {
                    Services = { DataTransmissionService.BindService(this) },
                    Ports = { new ServerPort(hostName, _port, ServerCredentials.Insecure), new ServerPort(hostName, _portSSL, ServerCredentials.Insecure) }
                };
            }
        }

        public Task ShutdownAsync()
        {
            if (_port != -1 || _portSSL == -1)
            {
                _logger.LogInformation($"Broker is shutdown");
                return _server.ShutdownAsync();
            }

            return Task.CompletedTask;
        }

        public void Start()
        {
            if (_port == -1 && _portSSL == -1)
            {
                _logger.LogError($"Broker exposing PORT is not configured. Configure port at {EnvironmentConstants.BrokerPort} and {EnvironmentConstants.BrokerConnectionSSLPort} Variables. Restart Vortex to take effect after the change");
                return;
            }

            _server.Start();

            if (_port != -1)
                _logger.LogInformation($"Broker listening on port {_port}");

            if (_portSSL != -1)
                _logger.LogInformation($"Broker listening on port {_portSSL} SSL");

        }

        public override Task<ConnectionResponse> Connect(ConnectionRequest request, ServerCallContext context)
        {
            var headers = context.RequestHeaders;
            var connectionType = Enum.Parse<ApplicationConnectionTypes>(request.ApplicationType.ToString());

            var connectionRequest = new ClientConnectionRequest()
            {
                Address = request.Address,
                AppKey = request.AppKey,
                Application = request.Application,
                ApplicationType = connectionType,
                ClientHost = context.Peer,
                ConnectedNode = _nodeConfiguration.NodeId
            };

            // Look for the Authorization header
            var authHeader = headers.FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase));
            if (authHeader != null)
            {
                connectionRequest.AppSecret = authHeader.Value;
            }

            if (connectionType == ApplicationConnectionTypes.Consumption)
            {
                // check in case the value is null
                // In case of NULL, in EstablishConnection, store the default value from Application
                if (request.SubscriptionModes == ConnectionSubscriptionModes.UndefinedValue)
                    connectionRequest.SubscriptionMode = null;
                else
                    connectionRequest.SubscriptionMode = Enum.Parse<SubscriptionModes>(request.SubscriptionModes.ToString());

                if (request.SubscriptionTypes == ConnectionSubscriptionTypes.Null)
                    connectionRequest.SubscriptionType = null;
                else
                    connectionRequest.SubscriptionType = Enum.Parse<SubscriptionTypes>(request.SubscriptionTypes.ToString());

                if (request.SubscriptionInitialPosition == ConnectionReadInitialPositions.Undefined)
                    connectionRequest.ReadInitialPosition = null;
                else
                    connectionRequest.ReadInitialPosition = Enum.Parse<ReadInitialPositions>(request.SubscriptionInitialPosition.ToString());
            }

            else
            {
                if (request.ProductionInstanceTypes == ConnectionProductionInstanceTypes.Unknown)
                    connectionRequest.ProductionInstanceType = null;
                else
                    connectionRequest.ProductionInstanceType = Enum.Parse<ProductionInstanceTypes>(request.ProductionInstanceTypes.ToString());
            }

            var result = _clientCommunicationService.EstablishConnection(connectionRequest);

            return Task.FromResult(new ConnectionResponse()
            {
                ClientId = result.ClientId.ToString(),
                Message = result.Message,
                Status = Enum.Parse<Statuses>(result.Status.ToString()),

                // Server info
                VortexServerName = SystemProperties.Name,
                VortexServerVersion = SystemProperties.Version,
            });
        }

        public override Task<DisconnectionResponse> Disconnect(DisconnectionRequest request, ServerCallContext context)
        {
            var headers = context.RequestHeaders;
            var connectionType = Enum.Parse<ApplicationConnectionTypes>(request.ApplicationType.ToString());

            var disconnectionRequest = new ClientDisconnectionRequest()
            {
                Address = request.Address,
                AppKey = request.AppKey,
                Application = request.Application,
                ApplicationType = connectionType,
                ClientId = request.ClientId,
                ClientHost = context.Peer
            };

            var authHeader = headers.FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase));
            if (authHeader != null)
            {
                disconnectionRequest.AppSecret = authHeader.Value;
            }

            var response = _clientCommunicationService.CloseConnection(disconnectionRequest);

            return Task.FromResult(new DisconnectionResponse()
            {
                ClientId = response.ClientId.ToString(),
                Message = response.Message,
                Status = Enum.Parse<Statuses>(response.Status.ToString()),
            });
        }

        public override Task<ConnectionResponse> Heartbeat(HeartbeatRequest request, ServerCallContext context)
        {
            var headers = context.RequestHeaders;
            TokenDetails tokenDetails = new TokenDetails() { AppKey = request.AppKey, AppSecret = "" };
            var authHeader = headers.FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase));
            if (authHeader != null)
            {
                tokenDetails.AppSecret = authHeader.Value;
            }

            if (Guid.TryParse(request.ClientId, out Guid clientId) != true)
                clientId = Guid.Empty;

            var response = _clientCommunicationService
                .HeartbeatConnection(Guid.Parse(request.ClientId), context.Peer, request.Application, request.Address, tokenDetails);

            return Task.FromResult(new ConnectionResponse()
            {
                ClientId = response.ClientId.ToString(),
                Message = response.Message,
                Status = Enum.Parse<Statuses>(response.Status.ToString()),

                // Server info
                VortexServerName = SystemProperties.Name,
                VortexServerVersion = SystemProperties.Version,
            });
        }
    }
}
