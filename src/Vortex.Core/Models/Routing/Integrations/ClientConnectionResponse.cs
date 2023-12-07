using Vortex.Core.Models.Routing.Common;

namespace Vortex.Core.Models.Routing.Integrations
{
    public class ClientConnectionResponse
    {
        public Guid ClientId { get; set; }

        public ConnectionStatuses Status { get; set; }
        public string Message { get; set; }


        // Server information
    }
}
