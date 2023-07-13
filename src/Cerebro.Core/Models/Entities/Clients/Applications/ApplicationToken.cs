using Cerebro.Core.Models.Common;
using Cerebro.Core.Models.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cerebro.Core.Models.Entities.Clients.Applications
{
    public class ApplicationToken : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("Applications")]
        public int ApplicationId { get; set; }

        public CryptographyTypes CryptographyType { get; set; }

        
        [JsonIgnore]
        public string? Secret { get; set; }

        public DateTimeOffset ExpireDate { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset IssuedDate { get; set; }
    }
}
