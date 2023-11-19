using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Utilities.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cerebro.Core.Models.Dtos.Clients
{
    public class ClientConnectionRequest
    {
        [Required]
        public string ApplicationName { get; set; }

        [Required]
        [AddressRegexValidation(ErrorMessage = "Address should start with / and should not contain letters, numbers and underscoor and dash")]

        public string Address { get; set; }

        public ApplicationConnectionTypes ApplicationConnectionType { get; set; }

        // In case of Production
        public ProductionInstanceTypes? ProductionInstanceType { get; set; }

        // In case of Consumption
        public SubscriptionTypes? SubscriptionType { get; set; }
        public SubscriptionModes? SubscriptionMode { get; set; }
        public ReadInitialPositions? ReadInitialPosition { get; set; }
    }
}
