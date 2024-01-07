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
        public ConsumptionSettings? ConsumptionSettings { get; set; }


        // NodeId, where the client is connected
        public string ConnectedNode { get; set; }
    }
}
