using Cerebro.Core.Models.Common.Clients.Applications;

namespace Cerebro.Core.Models.Entities.Clients.Applications
{
    public class ApplicationSettings
    {
        public bool IsAuthorizationEnabled { get; set; }
        public bool IsConnectionAllowedForAnyAddress { get; set; }

        public List<string> PublicIpRange { get; set; }
        public List<string> PrivateIpRange { get; set; }


        public ProductionInstanceTypes DefaultProductionInstanceType { get; set; }
        public SubscriptionTypes DefaultSubscriptionType { get; set; }
        public SubscriptionModes DefaultSubscriptionMode { get; set; }
        public ReadInitialPositions DefaultReadInitialPosition { get; set; }
    }
}
