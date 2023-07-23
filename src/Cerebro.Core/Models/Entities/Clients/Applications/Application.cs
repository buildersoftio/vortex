using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Entities.Base;

namespace Cerebro.Core.Models.Entities.Clients.Applications
{
    public class Application : BaseEntity
    {
        // AutoID
        public int Id { get; set; }

        public string Name { get; set; } // name is unique
        public string Description { get; set; }

        public ApplicationSettings Settings { get; set; }
    }
}
