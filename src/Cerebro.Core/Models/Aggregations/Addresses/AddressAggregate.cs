using Cerebro.Core.Models.Entities.Addresses;

namespace Cerebro.Core.Models.Aggregations.Addresses
{
    public class AddressAggregate
    {
        public Address Address { get; set; }
        public AddressSettings AddressSettings { get; set; }
    }
}
