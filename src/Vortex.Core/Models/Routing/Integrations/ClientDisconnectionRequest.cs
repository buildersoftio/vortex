using Vortex.Core.Models.Common.Clients.Applications;

namespace Vortex.Core.Models.Routing.Integrations
{
    public class ClientDisconnectionRequest
    {
        public string Application { get; set; }
        public string Address { get; set; }
        public ApplicationConnectionTypes ApplicationType { get; set; }


        public string ClientId { get; set; }


        public string AppKey { get; set; }
        public string AppSecret { get; set; }

        public string ClientHost { get; set; }
    }
}
