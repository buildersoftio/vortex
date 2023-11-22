using ApplicationIntegration;
using Vortex.Core.Abstractions.Clients;
using Vortex.Core.Utilities.Consts;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Vortex.Core.Abstractions.Services.Routing;
using Vortex.Core.Models.Common.Clients.Applications;

namespace Vortex.Grpc.Servers
{
    public class BrokerClientIntegrationServer : DataTransmissionService.DataTransmissionServiceBase, IClientIntegrationServer
    {
        private readonly ILogger<BrokerClientIntegrationServer> _logger;
        private readonly IClientCommunicationService _clientCommunicationService;

        private readonly Server _server;
        private readonly int _port;
        private readonly int _portSSL;

        public BrokerClientIntegrationServer(ILogger<BrokerClientIntegrationServer> logger, IClientCommunicationService clientCommunicationService)
        {
            _logger = logger;
            _clientCommunicationService = clientCommunicationService;

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

            var connectionRequest = new Core.Models.Routing.Integrations.ClientConnectionRequest()
            {
                Address = request.Address,
                AppKey = request.AppKey,
                Application = request.Application,
                ApplicationType = connectionType,
                ClientHost = context.Peer
            };

            // Look for the Authorization header
            var authHeader = headers.FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase));
            if (authHeader != null)
            {
                connectionRequest.AppSecret = authHeader.Value;
            }

            if (connectionType == ApplicationConnectionTypes.Consumption)
            {
                connectionRequest.SubscriptionMode = Enum.Parse<SubscriptionModes>(request.SubscriptionModes.ToString());
                connectionRequest.SubscriptionType = Enum.Parse<SubscriptionTypes>(request.SubscriptionTypes.ToString());
                connectionRequest.ReadInitialPosition = Enum.Parse<ReadInitialPositions>(request.SubscriptionInitialPosition.ToString());
            }
            else
            {
                connectionRequest.ProductionInstanceType = Enum.Parse<ProductionInstanceTypes>(request.ProductionInstanceTypes.ToString());
            }

            var result = _clientCommunicationService.EstablishConnection(connectionRequest);

            return Task.FromResult(new ConnectionResponse()
            {
                ClientId = result.ClientId.ToString(),
                Message = result.Message,
                Status = Enum.Parse<Statuses>(result.Status.ToString()),

                // Server infos
                VortexServerName = SystemProperties.Name,
                VortexServerVersion = SystemProperties.Version,
            });
        }

        public override Task<DisconnectionResponse> Disconnect(DisconnectionRequest request, ServerCallContext context)
        {
            // to implement

            return base.Disconnect(request, context);
        }
    }
}
