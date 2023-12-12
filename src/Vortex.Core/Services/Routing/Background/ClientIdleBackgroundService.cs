using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Abstractions.Services.Routing;
using Vortex.Core.Models.BackgroundRequests;
using Vortex.Core.Models.BackgroundTimerRequests;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Repositories;
using Vortex.Core.Services.ServerStates;

namespace Vortex.Core.Services.Routing.Background
{
    public class ClientIdleBackgroundService(
        ILogger<ClientIdleBackgroundService> logger,
        NodeConfiguration nodeConfiguration,
        IClientConnectionService clientConnectionService,
        IBackgroundQueueService<ClientConnectionBackgroundRequest> clusterClientConnectionService,
        IClusterStateRepository clusterStateRepository,
        IAddressRepository addressRepository,
        IApplicationRepository applicationRepository,
        IClientCommunicationService clientCommunicationService) : TimedBackgroundServiceBase<ClientIdleTimerRequest>(GetPeriod(nodeConfiguration))
    {
        private readonly ILogger<ClientIdleBackgroundService> _logger = logger;
        private readonly NodeConfiguration _nodeConfiguration = nodeConfiguration;
        private readonly IClientConnectionService _clientConnectionService = clientConnectionService;
        private readonly IBackgroundQueueService<ClientConnectionBackgroundRequest> _clusterClientConnectionService = clusterClientConnectionService;
        private readonly IClusterStateRepository _clusterStateRepository = clusterStateRepository;

        private readonly IAddressRepository _addressRepository = addressRepository;
        private readonly IApplicationRepository _applicationRepository = applicationRepository;
        private readonly IClientCommunicationService _clientCommunicationService = clientCommunicationService;

        private static TimeSpan GetPeriod(NodeConfiguration nodeConfiguration)
        {
            return new TimeSpan(0, 0, nodeConfiguration.IdleClientConnectionInterval);
        }

        public override void OnTimer_Callback(object? state)
        {
            int deadConnections = 0;
            var currentTime = DateTimeOffset.UtcNow;

            var connections = _clientConnectionService
                .GetConnectedClientConnections();

            foreach (var conn in connections!)
            {
                // here for each client we check for dead connections.
                // check if the heartbeat is older than connection timeout - close connection for that client connection.
                // The background service can disconnect only clients that are connected in this node, to update other clients from different nodes in cluster
                //      is client's node leaders to do the update.

                foreach (var clientCon in conn.HostsHistory.Where(x => x.Value.IsConnected == true && x.Value.ConnectedNode == _nodeConfiguration.NodeId))
                {
                    var lastHeartbeat = clientCon.Value.LastHeartbeatDate;

                    if (lastHeartbeat == null)
                    {
                        deadConnections++;
                        conn.HostsHistory[clientCon.Key].IsConnected = false;
                        continue;
                    }

                    if (currentTime - lastHeartbeat > TimeSpan.FromSeconds(_nodeConfiguration.IdleClientConnectionTimeout))
                    {
                        deadConnections++;
                        conn.HostsHistory[clientCon.Key].IsConnected = false;

                        InformOtherClusters(conn, clientCon);
                    }
                }

                // isConnected should be false in all hosts are disconnected
                if (conn.HostsHistory.All(pair => pair.Value.IsConnected != true))
                {
                    if (conn.ApplicationConnectionType == ApplicationConnectionTypes.Production)
                        _clientCommunicationService.DeleteClientProducerFromCache(conn.Id);

                    conn.IsConnected = false;
                }

                // update client connection
                conn.UpdatedBy = "Idle_BackgroundService";
                _clientConnectionService.UpdateClientConnection(conn);

                //
                // TODO: Inform other nodes that this connection is closed.
                //
                //
            }

            if (deadConnections > 0)
                _logger.LogInformation($"{deadConnections} dead or idle connections are closed automatically, due to being inactive");
        }

        private void InformOtherClusters(Models.Entities.Clients.Applications.ClientConnection conn, KeyValuePair<string, ApplicationHost> clientCon)
        {
            if (_clusterStateRepository.GetCluster().Status == Models.Common.Clusters.ClusterStatus.OnlineStandalone)
                return;


            // send request to other nodes. for dead clients.
            var address = _addressRepository.GetAddressById(conn.AddressId);
            if (address == null)
                return;

            var application = _applicationRepository.GetApplication(conn.ApplicationId);
            if (application == null)
                return;

            _clusterClientConnectionService.EnqueueRequest(new ClientConnectionBackgroundRequest()
            {
                RequestState = ClientConnectionRequestState.ClientDisconnection,

                Application = application.Name,
                Address = address.Name,
                ApplicationType = conn.ApplicationConnectionType,
                ClientId = conn.Id.ToString(),

                //TODO: check how to add credentials here.. this is really important
                Credentials = new TokenDetails() { AppKey = "", AppSecret = "" },

                ClientHost = clientCon.Key,
                ConnectedNode = _nodeConfiguration.NodeId
            });

        }
    }
}
