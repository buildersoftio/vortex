using Cerebro.Core.Models.Common.Addresses;
using Cerebro.Core.Models.Entities.Base;

namespace Cerebro.Core.Models.Entities.Addresses
{
    public class Address : BaseEntity
    {
        // ID is auto incremented
        public int Id { get; set; }

        // if Name is "root/system/audit/logging"; alias should be "root_audit_logging", "system_audit_logging" or "root_sys_audit"
        public string Alias { get; set; } // Alias is as short description in which can be used for the address. e.g.,"something", it should be unique.
        public string Name { get; set; } // name will be in rules like "root/something" or "something"...

        public AddressStatuses AddressStatus { get; set; }

        public AddressSettings Settings { get; set; }
    }
}
