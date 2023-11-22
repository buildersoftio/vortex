using Vortex.Core.Models.Common;
using Vortex.Core.Models.Entities.Base;

namespace Vortex.Core.Models.Entities.Clients.Applications
{
    public class ApplicationToken : BaseEntity
    {
        // Id is being used as API Key (Application Token Key)
        public Guid Id { get; set; }

        // foreign key
        public int ApplicationId { get; set; }

        public CryptographyTypes CryptographyType { get; set; }

        
        public string? HashedSecret { get; set; }

        public DateTimeOffset ExpireDate { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset IssuedDate { get; set; }
    }
}
