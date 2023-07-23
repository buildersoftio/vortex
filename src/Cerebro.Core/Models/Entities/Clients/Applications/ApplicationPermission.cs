using Cerebro.Core.Models.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cerebro.Core.Models.Entities.Clients.Applications
{
    public class ApplicationPermission : BaseEntity
    {
        public int Id { get; set; }
        // Index, Unique Id; is acting as primary key
        public int ApplicationId { get; set; }


        /// <summary>
        /// The key will determinate if this application can read or write.
        /// e.g., with addresses; READ_ADDRESSES: root/*:{*} 
        ///                       WRITE_ADDRESSES: *:{*}
        ///                       OR
        ///                       READ_ADDRESSES: system/logging/incoming:{*}
        ///                       {*} it means the app can read all partitions, other examples can be {1,2}, {1,4} etc.
        ///                       
        /// There will be some default permissions when we create a new application.
        /// </summary>
        public Dictionary<string, string>? Permissions { get; set; }
    }
}
