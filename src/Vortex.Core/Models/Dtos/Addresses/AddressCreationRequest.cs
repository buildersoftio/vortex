using Vortex.Core.Models.Common.Addresses;
using Vortex.Core.Utilities.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Vortex.Core.Models.Dtos.Addresses
{
    public class AddressCreationRequest
    {
        [Required]
        [StringLength(30)]
        [NameRegexValidation(ErrorMessage = "Address alias can contain only letters, numbers, underscores and dashes")]
        public string Alias { get; set; }

        [Required]
        [AddressRegexValidation(ErrorMessage = "Address should start with / and should contain letters, numbers, underscoor, dash. Also, address should not end with /")]
        public string Name { get; set; }

        public AddressSettings Settings { get; set; }
    }
}
