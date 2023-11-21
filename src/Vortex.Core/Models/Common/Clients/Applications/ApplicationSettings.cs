using System.Text.Json.Serialization;

namespace Vortex.Core.Models.Common.Clients.Applications
{
    public class ApplicationSettings
    {
        public ApplicationScope Scope { get; set; }

        public bool IsAuthorizationEnabled { get; set; }

        public HashSet<string> PublicIpRange { get; set; }
        public HashSet<string> PrivateIpRange { get; set; }

        public ProductionInstanceTypes DefaultProductionInstanceType { get; set; }
        public SubscriptionTypes DefaultSubscriptionType { get; set; }
        public SubscriptionModes DefaultSubscriptionMode { get; set; }
        public ReadInitialPositions DefaultReadInitialPosition { get; set; }

        public ApplicationSettings()
        {
            Scope = ApplicationScope.SingleScope;
            PublicIpRange = new HashSet<string>();
            PrivateIpRange = new HashSet<string>();
        }
    }
}
