using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Entities.Clients.Applications;
using Cerebro.Core.Utilities.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cerebro.Core.Models.Dtos.Applications
{
    public class ApplicationDto
    {
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        [NameRegexValidation(ErrorMessage = "Application name can contain only letters, numbers, underscores and dashes")]
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
            Id = application.Id;
            Name = application.Name;
            Description = application.Description;
            Settings = application.Settings;
        }
    }
}
