using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Entities.Base;

namespace Vortex.Core.Models.Entities.Clients.Applications
{
    public class ClientConnection : BaseEntity
    {
        public Guid Id { get; set; }

        // foreign key for Applications
        public int ApplicationId { get; set; }

        // foreign key for Addresses
        public int AddressId { get; set; }

        public ApplicationConnectionTypes ApplicationConnectionType { get; set; }

        public DateTimeOffset? FirstConnectionDate { get; set; }
        public DateTimeOffset? LastConnectionDate { get; set; }

        public bool IsConnected { get; set; }


        public ProductionInstanceTypes ProductionInstanceType { get; set; }
        public ConsumptionSettings ConsumptionSettings { get; set; }

        public List<string>? ConnectedHosts { get; set; }
        public Dictionary<string, ApplicationHost> HostsHistory { get; set; }

        public ClientConnection()
        {
            HostsHistory = new Dictionary<string, ApplicationHost>();
        }
    }
}
