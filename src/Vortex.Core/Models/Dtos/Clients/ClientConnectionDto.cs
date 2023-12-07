using Vortex.Core.Models.Common.Clients.Applications;

namespace Vortex.Core.Models.Dtos.Clients
{
    public class ClientConnectionDto
    {
        public Guid Id { get; set; }

        // foreign key for Applications
        public string? ApplicationName { get; set; }

        public string? Address { get; set; }

        public ApplicationConnectionTypes ApplicationConnectionType { get; set; }

        public DateTimeOffset? FirstConnectionDate { get; set; }
        public DateTimeOffset? LastConnectionDate { get; set; }

        public bool IsConnected { get; set; }

        public ProductionInstanceTypes? ProductionInstanceType { get; set; }
        public SubscriptionTypes? SubscriptionType { get; set; }
        public SubscriptionModes? SubscriptionMode { get; set; }
        public ReadInitialPositions? ReadInitialPosition { get; set; }

        public List<string>? ConnectedHosts { get; set; }
        public Dictionary<string, ApplicationHost> HostsHistory { get; set; }

        public ClientConnectionDto()
        {
            HostsHistory = new Dictionary<string, ApplicationHost>();
        }
    }
}
