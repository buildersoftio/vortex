using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Entities.Clients.Applications;

namespace Cerebro.Core.Models.Dtos.Applications
{
    public class ApplicationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
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
