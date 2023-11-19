using ApplicationIntegration;
using Vortex.Core.Abstractions.Clients;
using Vortex.Core.Utilities.Consts;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Vortex.Grpc.Servers
{
    public class ClientServer : DataTransmissionService.DataTransmissionServiceBase, IClientIntegrationServer
    {
        private readonly ILogger<ClientServer> _logger;
        private readonly Server _server;
        private readonly int _port;

        public ClientServer(ILogger<ClientServer> logger)
        {
            _logger = logger;

            if (Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerPort) != null)
                _port = Convert.ToInt32(Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerPort));
            else
                _port = -1;

            string? hostName = Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerHost);
            hostName ??= "localhost";

            _server = new Server()
            {
                Services = { DataTransmissionService.BindService(this) },
                Ports = { new ServerPort(hostName, _port, ServerCredentials.Insecure) }
            };

        }

        public Task ShutdownAsync()
        {
            if (_port != -1)
            {
                _logger.LogInformation($"Broker is shutdown");
                return _server.ShutdownAsync();
            }

            return Task.CompletedTask;
        }

        public void Start()
        {
            if (_port == -1)
            {
                _logger.LogError($"Broker exposing PORT is not configured. Configure port at {EnvironmentConstants.BrokerPort} and {EnvironmentConstants.BrokerConnectionSSLPort} Variables. Restart Cerebro to take effect after the change");

                return;
            }
            _server.Start();
            _logger.LogInformation($"Broker listening on port {_port}");

        }


        public override Task<ConnectionResponse> Connect(ConnectionRequest request, ServerCallContext context)
        {


            return base.Connect(request, context);
        }

        public override Task<DisconnectionResponse> Disconnect(DisconnectionRequest request, ServerCallContext context)
        {
            // to implement

            return base.Disconnect(request, context);
        }
    }
}
