using Vortex.Core.Models.Common.Clients.Applications;

namespace Vortex.Core.Models.Routing.Integrations
{
    public class ClientConnectionRequest
    {
        public string Application { get; set; }
        public string Address { get; set; }
        public ApplicationConnectionTypes ApplicationType { get; set; }

        public string AppKey { get; set; }
        public string AppSecret { get; set; }

        public string ClientHost { get; set; }


        /// Settings
        // In case of Production
        public ProductionInstanceTypes? ProductionInstanceType { get; set; }

        // In case of Consumption
        public SubscriptionTypes? SubscriptionType { get; set; }
        public SubscriptionModes? SubscriptionMode { get; set; }
        public ReadInitialPositions? ReadInitialPosition { get; set; }
    }
}
