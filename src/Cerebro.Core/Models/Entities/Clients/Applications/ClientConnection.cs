using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Entities.Base;

namespace Cerebro.Core.Models.Entities.Clients.Applications
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

        public SubscriptionTypes SubscriptionType { get; set; }
        public SubscriptionModes SubscriptionMode { get; set; }
        public ReadInitialPositions ReadInitialPosition { get; set; }

        public List<string>? ConnectedIPs { get; set; }

    }
}
