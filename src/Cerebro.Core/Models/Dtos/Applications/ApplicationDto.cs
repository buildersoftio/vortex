using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Entities.Clients.Applications;
using Cerebro.Core.Utilities.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cerebro.Core.Models.Dtos.Applications
{
    public class ApplicationDto
    {
        [Required]
        [StringLength(30)]
        [ApplicationNameRegexValidation(ErrorMessage = "Application name should contain only letters, numbers, underscores and dashes can be used")]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        public ApplicationSettings Settings { get; set; }

        public ApplicationDto()
        {

        }

        public ApplicationDto(Application application)
        {
            Name = application.Name;
            Description = application.Description;
            Settings = application.Settings;
        }
    }
}
