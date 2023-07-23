using Cerebro.Core.Models.Entities.Base;

namespace Cerebro.Core.Models.Entities.Addresses
{
    public class Address : BaseEntity
    {
        // ID is auto incremented
        public int Id { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; } // name will be in rules like "root/something" or "something"...
        public AddressSettings Settings { get; set; }
        public int SchemaId { get; set; }
    }
}
