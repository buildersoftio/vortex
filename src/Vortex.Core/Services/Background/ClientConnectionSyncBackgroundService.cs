using Microsoft.Extensions.Logging;
using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.IO.Services;
using Vortex.Core.Models.BackgroundRequests;

namespace Vortex.Core.Services.Background
{
    public class ClientConnectionSyncBackgroundService : BackgroundQueueServiceBase<ClientConnectionBackgroundRequest>
    {
        private readonly ILogger<ClientConnectionSyncBackgroundService> _logger;
        private readonly IClusterStateRepository _clusterStateRepository;

        public ClientConnectionSyncBackgroundService(ILogger<ClientConnectionSyncBackgroundService> logger,
            IClusterStateRepository clusterStateRepository,
            ITemporaryIOService temporaryIOService) : base("clientConnection_", temporaryIOService)
        {
            _logger = logger;
            _clusterStateRepository = clusterStateRepository;
        }


        public override void Handle(ClientConnectionBackgroundRequest request)
        {

            switch (request.RequestState)
            {
                case ClientConnectionRequestState.EstablishConnection:
                    Request_EstablishConnection(request);
                    break;
                case ClientConnectionRequestState.HeartbeatConnection:
                    Request_HeartbeatConnection(request);
                    break;
                case ClientConnectionRequestState.ClientDisconnection:
                    Request_ClientDisconnection(request);
                    break;
                default:
                    break;
            }
        }

        private async void Request_EstablishConnection(ClientConnectionBackgroundRequest request)
        {
            _logger.LogInformation($"Requesting Client connection to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestClientConnectionRegister(request.Application,
                        request.Address, request.ApplicationType, request.Credentials, request.ClientHost, request.ConnectedNode, request.ProductionInstanceType, request.ConsumptionSettings);

                    if (success == true)
                        continue;

                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Client connection failed at {nodeClient.Key}, request is saved temporary");
                StoreFailedRequestInFile(request, nodeClient.Key);
            }
        }

        private async void Request_HeartbeatConnection(ClientConnectionBackgroundRequest request)
        {
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestClientConnectionHeartbeat(request.Application,
                        request.Address, request.ApplicationType, request.Credentials, request.ClientHost, request.ConnectedNode, request.ClientId);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Client connection heartbeat failed at {nodeClient.Key}, request is saved temporary");
                StoreFailedRequestInFile(request, nodeClient.Key);
            }
        }

        private async void Request_ClientDisconnection(ClientConnectionBackgroundRequest request)
        {
            _logger.LogInformation($"Requesting Client disconnection to neighbor nodes");

            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestClientConnectionUnregister(request.Application,
                        request.Address, request.ApplicationType, request.Credentials, request.ClientHost, request.ConnectedNode, request.ClientId);
                    if (success == true)
                        continue;

                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Client disconnection failed at {nodeClient.Key}, request is saved temporary");
                StoreFailedRequestInFile(request, nodeClient.Key);
            }
        }

        private void StoreFailedRequestInFile(ClientConnectionBackgroundRequest request, string nodeId)
        {
            StoreFailedRequest(new ClientConnectionBackgroundRequest()
            {
                RequestState = request.RequestState,
                NodeId = nodeId,
                ProductionInstanceType = request.ProductionInstanceType,
                Credentials = request.Credentials,
                Address = request.Address,
                Application = request.Application,
                ApplicationType = request.ApplicationType,
                ClientHost = request.ClientHost,
                ClientId = request.ClientId,
                ConnectedNode = request.ConnectedNode
            });
        }
    }
}
