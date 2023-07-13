using Cerebro.Core.Models.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cerebro.Core.Models.Entities.Clients.Applications
{
    public class Permission : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// The key will determinate if this application can read or write.
        /// e.g., with addresses; READ_ADDRESSES: root/*:{*} 
        ///                       WRITE_ADDRESSES: *:{*}
        ///                       OR
        ///                       READ_ADDRESSES: system/logging/incoming:{*}
        ///                       {*} it means the app can read all partitions, other examples can be {1,2}, {1,4} etc.
        /// </summary>
        public string Key { get; set; } // Key = "addresses", "networking", "pipelines", "user-management" etc....
        public string Value { get; set; } // value = "*", or "tenantName01;tenantName02";
    }
}
