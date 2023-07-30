using Cerebro.Core.Models.Common.Addresses;
using Cerebro.Core.Utilities.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cerebro.Core.Models.Dtos.Addresses
{
    public class AddressCreationRequest
    {
        [Required]
        [StringLength(30)]
        [NameRegexValidation(ErrorMessage = "Address alias can contain only letters, numbers, underscores and dashes")]
        public string Alias { get; set; }

        [Required]
        [AddressRegexValidation(ErrorMessage = "Address should start with / and should not contain letters, numbers and underscoor and dash")]
        public string Name { get; set; }

        public AddressSettings Settings { get; set; }
    }
}
