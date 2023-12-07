using Vortex.Core.Abstractions.Background;
using Vortex.Core.Models.Common.Clients.Applications;

namespace Vortex.Core.Models.BackgroundRequests
{
    public class ClientConnectionBackgroundRequest : RequestBase
    {
        public ClientConnectionRequestState RequestState { get; set; }

        // core properties
        public string Application { get; set; }
        public string Address { get; set; }
        public ApplicationConnectionTypes ApplicationType { get; set; }

        public TokenDetails Credentials { get; set; }

        public string ClientHost { get; set; }

        /// Settings
        public ProductionInstanceTypes? ProductionInstanceType { get; set; }
        public SubscriptionTypes? SubscriptionType { get; set; }
        public SubscriptionModes? SubscriptionMode { get; set; }
        public ReadInitialPositions? ReadInitialPosition { get; set; }

        public string ConnectedNode { get; set; }


        // in case of heartbeat
        public string ClientId { get; set; }

        public ClientConnectionBackgroundRequest()
        {
            RequestState = ClientConnectionRequestState.Unknown;
        }
    }

    public enum ClientConnectionRequestState
    {
        Unknown,
        EstablishConnection,
        HeartbeatConnection,
        ClientDisconnection
    }
}
