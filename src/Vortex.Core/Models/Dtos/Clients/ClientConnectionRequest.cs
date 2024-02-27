using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Utilities.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Vortex.Core.Models.Dtos.Clients
{
    public class ClientConnectionRequest
    {
        [Required]
        public string ApplicationName { get; set; }

        [Required]
        [AddressRegexValidation(ErrorMessage = "Address should start with / and should not contain letters, numbers and underscore and dash")]

        public string Address { get; set; }

        public ApplicationConnectionTypes ApplicationConnectionType { get; set; }

        // In case of Production
        public ProductionInstanceTypes? ProductionInstanceType { get; set; }

        // In case of Consumption
        public string? SubscriptionName { get; set; }
        public ConsumptionSettings? ConsumptionSettings { get; set; }
    }
}
