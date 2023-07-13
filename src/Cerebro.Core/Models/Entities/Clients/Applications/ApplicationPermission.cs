using Cerebro.Core.Models.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cerebro.Core.Models.Entities.Clients.Applications
{
    public class ApplicationPermission : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("Applications")]
        public int ApplicationId { get; set; }

        [ForeignKey("Permissions")]
        public long PermissionId { get; set; }
    }
}
