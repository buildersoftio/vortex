using System.ComponentModel.DataAnnotations;

namespace Cerebro.Core.Models.Common.Clients.Applications
{
    public class ApplicationSettings
    {
        public bool IsAuthorizationEnabled { get; set; }
        public bool IsConnectionAllowedForAnyAddress { get; set; }

        public HashSet<string> PublicIpRange { get; set; }
        public HashSet<string> PrivateIpRange { get; set; }

        public ProductionInstanceTypes DefaultProductionInstanceType { get; set; }
        public SubscriptionTypes DefaultSubscriptionType { get; set; }
        public SubscriptionModes DefaultSubscriptionMode { get; set; }
        public ReadInitialPositions DefaultReadInitialPosition { get; set; }

        public ApplicationSettings()
        {
            PublicIpRange = new HashSet<string>();
            PrivateIpRange = new HashSet<string>();
        }
    }
}
