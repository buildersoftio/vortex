using Cerebro.Core.Models.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cerebro.Core.Models.Entities.Addresses
{
    public class Address : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; } // name will be in rules like "root/something" or "something"...

        public int SchemaId { get; set; }
    }
}
