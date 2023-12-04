using Microsoft.Extensions.Logging;
using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Models.BackgroundTimerRequests;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Entities.Clients.Applications;

namespace Vortex.Core.Services.Routing.Background
{
    public class ClientIdleBackgroundService(
        ILogger<ClientIdleBackgroundService> logger,
        NodeConfiguration nodeConfiguration,
        IClientConnectionService clientConnectionService) : TimedBackgroundServiceBase<ClientIdleTimerRequest>(GetPeriod(nodeConfiguration))
    {
        private readonly ILogger<ClientIdleBackgroundService> _logger = logger;
        private readonly NodeConfiguration _nodeConfiguration = nodeConfiguration;
        private readonly IClientConnectionService _clientConnectionService = clientConnectionService;

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

                foreach (var clientCon in conn.HostsHistory.Where(x => x.Value.IsConnected == true))
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
                    }
                }

                // isConnected should be false in all hosts are disconnected
                if (conn.HostsHistory.All(pair => pair.Value.IsConnected != true))
                    conn.IsConnected = false;

                // update client connection
                _clientConnectionService.UpdateClientConnection(conn);
            }

            if (deadConnections > 0)
                _logger.LogInformation($"{deadConnections} dead or idle connections are closed automatically, due to being inactive");
        }
    }
}
