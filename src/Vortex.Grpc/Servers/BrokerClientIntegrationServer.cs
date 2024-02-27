using ApplicationIntegration;
using Vortex.Core.Abstractions.Clients;
using Vortex.Core.Utilities.Consts;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Vortex.Core.Abstractions.Services.Routing;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Routing.Integrations;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Data;
using Vortex.Core.Abstractions.Services;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Vortex.Grpc.Servers
{
    public class BrokerClientIntegrationServer : DataTransmissionService.DataTransmissionServiceBase, IClientIntegrationServer
    {
        private readonly ILogger<BrokerClientIntegrationServer> _logger;
        private readonly IClientCommunicationService _clientCommunicationService;
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly IClientConnectionService _clientConnectionService;

        private readonly Server _server;
        private readonly int _port;
        private readonly int _portSSL;

        public BrokerClientIntegrationServer(
            ILogger<BrokerClientIntegrationServer> logger,
            IClientCommunicationService clientCommunicationService,
            NodeConfiguration nodeConfiguration,
            IClientConnectionService clientConnectionService)
        {
            _logger = logger;
            _clientCommunicationService = clientCommunicationService;
            _nodeConfiguration = nodeConfiguration;
            _clientConnectionService = clientConnectionService;

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
            var connectionType = System.Enum.Parse<ApplicationConnectionTypes>(request.ApplicationType.ToString());

            var connectionRequest = new ClientConnectionRequest()
            {
                Address = request.Address,
                AppKey = request.AppKey,
                Application = request.Application,
                ApplicationType = connectionType,
                ClientHost = context.Peer,
                ConnectedNode = _nodeConfiguration.NodeId,
            };

            // Look for the Authorization header
            var authHeader = headers.FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase));
            if (authHeader != null)
                connectionRequest.AppSecret = authHeader.Value;

            if (connectionType == ApplicationConnectionTypes.Consumption)
            {
                // check in case the value is null
                // in case of NULL - in EstablishConnection, store the default value from Application

                if (request.ConsumptionSettings == null)
                    connectionRequest.ConsumptionSettings = null;
                else
                {
                    request.ConsumptionSettings = new ConnectionConsumptionSettings()
                    {
                        AutoCommitEntry = request.ConsumptionSettings.AutoCommitEntry,
                        ConnectionAcknowledgmentType = System.Enum.Parse<ConnectionAcknowledgmentTypes>(request.ConsumptionSettings.ConnectionAcknowledgmentType.ToString()),
                        ConnectionReadInitialPosition = System.Enum.Parse<ConnectionReadInitialPositions>(request.ConsumptionSettings.ConnectionReadInitialPosition.ToString())
                    };
                }

                if (request.SubscriptionName == null)
                    connectionRequest.SubscriptionName = null;
                else
                    connectionRequest.SubscriptionName = request.SubscriptionName;
            }
            else
            {
                if (request.ProductionInstanceTypes == ConnectionProductionInstanceTypes.Unknown)
                    connectionRequest.ProductionInstanceType = null;
                else
                    connectionRequest.ProductionInstanceType = System.Enum.Parse<ProductionInstanceTypes>(request.ProductionInstanceTypes.ToString());
            }

            var result = _clientCommunicationService.EstablishConnection(connectionRequest);

            return Task.FromResult(new ConnectionResponse()
            {
                ClientId = result.ClientId.ToString(),
                Message = result.Message,
                Status = System.Enum.Parse<Statuses>(result.Status.ToString()),

                // Server info
                VortexServerName = SystemProperties.Name,
                VortexServerVersion = SystemProperties.Version,
            });
        }

        public override Task<DisconnectionResponse> Disconnect(DisconnectionRequest request, ServerCallContext context)
        {
            var headers = context.RequestHeaders;
            var connectionType = System.Enum.Parse<ApplicationConnectionTypes>(request.ApplicationType.ToString());

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
                Status = System.Enum.Parse<Statuses>(response.Status.ToString()),
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
                Status = System.Enum.Parse<Statuses>(response.Status.ToString()),

                // Server info
                VortexServerName = SystemProperties.Name,
                VortexServerVersion = SystemProperties.Version,
            });
        }


        public override async Task<MessageStreamResponse> StreamMessage(IAsyncStreamReader<MessageRequest> requestStream, ServerCallContext context)
        {
            int messageCount = 0;
            try
            {
                while (await requestStream.MoveNext())
                {
                    messageCount++;

                    MessageRequest request = requestStream.Current;

                    var partitionMessage = new PartitionMessage()
                    {
                        MessageId = request.MessageId.ToByteArray(),
                        MessagePayload = request.MessagePayload.ToByteArray(),
                        MessageHeaders = new Dictionary<string, string>(request.MessageHeaders),
                        PartitionIndex = request.PartitionIndex,
                        HostApplication = context.Peer,
                        SourceApplication = request.SourceApplication,
                        SentDate = request.SentDate.ToDateTime(),
                        StoredDate = DateTime.Now
                    };

                    (bool success, int? partitionKey, string message) =
                        _clientCommunicationService.AcceptMessage(Guid.Parse(request.ClientId), new Span<PartitionMessage>(ref partitionMessage));
                }

                return new MessageStreamResponse()
                {
                    Success = true,
                    Message = $"Producing completed",
                    MessageCount = messageCount
                };
            }
            catch (Exception ex)
            {
                return new MessageStreamResponse()
                {
                    Success = false,
                    Message = $"Streaming of the messages failed, details:{ex.Message}",
                    MessageCount = messageCount
                };
            }
        }

        public override Task<MessageResponse> ProduceMessage(MessageRequest request, ServerCallContext context)
        {
            try
            {
                var partitionMessage = new PartitionMessage()
                {
                    MessageId = request.MessageId.ToByteArray(),
                    MessagePayload = request.MessagePayload.ToByteArray(),
                    MessageHeaders = new Dictionary<string, string>(request.MessageHeaders),
                    PartitionIndex = request.PartitionIndex,
                    HostApplication = context.Peer,
                    SourceApplication = request.SourceApplication,
                    SentDate = request.SentDate.ToDateTime(),
                    StoredDate = DateTime.Now
                };

                (bool success, int? partitionKey, string message) =
                    _clientCommunicationService.AcceptMessage(Guid.Parse(request.ClientId), new Span<PartitionMessage>(ref partitionMessage));

                return Task.FromResult(new MessageResponse()
                {
                    Success = success,
                    Message = message,
                    PartitionIndex = partitionKey!.Value,
                });

            }
            catch (Exception ex)
            {
                return Task.FromResult(new MessageResponse()
                {
                    Success = false,
                    Message = $"Streaming of the message failed, details:{ex.Message}",
                    PartitionIndex = -1,
                });
            }
        }

        public override async Task ConsumeMessage(ConsumeMessageRequest request, IServerStreamWriter<ConsumeMessageResponse> responseStream, ServerCallContext context)
        {
            // check token validity.

            var isClientIdValid = Guid.TryParse(request.ClientId, out Guid clientId);
            if (isClientIdValid != true)
                clientId = Guid.Empty;

            string appKey = request.ApiKey;
            string appSecret = "";

            var headers = context.RequestHeaders;
            var authHeader = headers.FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase));
            if (authHeader != null)
                appSecret = authHeader.Value;

            if (_clientCommunicationService.ValidateApplicationToken(clientId, appKey, appSecret) != true)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid app key or secret for this application"));


            var clientConnection = _clientConnectionService.GetClientConnection(clientId);
            if (clientConnection == null)
                throw new RpcException(new Status(StatusCode.Aborted, "Application Connection does not exists."));

            try
            {
                // read next messages
                List<int> partitions = request.ReadPartitions.ToList();
                int currentPartitionIndex = partitions[0];

                // read the same amount of data for each_partition;
                int readMessageCount = request.ReadMessageCount * partitions.Count;

                for (int i = 0; i < readMessageCount; i++)
                {
                    // read and send messages to client.
                    var messageRead = _clientCommunicationService.ConsumeNextMessage(clientId, clientConnection.AddressId, partitions[i], partitions[currentPartitionIndex]);
                    if (messageRead != null)
                    {
                        // map message to GRPC message.
                        await responseStream.WriteAsync(new ConsumeMessageResponse()
                        {
                            EntryId = messageRead.EntryId,
                            MessageHeaders = { messageRead.MessageHeader },
                            MessageId = ByteString.CopyFrom(messageRead.MessageId),
                            MessagePayload = ByteString.CopyFrom(messageRead.MessagePayload),
                            Partition = messageRead.PartitionId,
                            SentDate = messageRead.SentDate.ToTimestamp(),
                            SourceApplication = messageRead.SourceApplication
                        });
                    }

                    currentPartitionIndex++;
                    if (currentPartitionIndex == partitions.Count())
                        currentPartitionIndex = 0;
                }
            }
            catch (Exception)
            {
                _logger.LogError("Subscription {0} failed pulling data out, using client with id {1}", request.SubscriptionName, request.ClientId);
            }
        }
    }
}
