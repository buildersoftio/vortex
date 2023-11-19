using System.ComponentModel.DataAnnotations;

namespace Cerebro.Core.Models.Common.Addresses
{
    public class AddressPartitionSettings
    {
        [Range(1,20)]
        public int PartitionNumber { get; set; }
        public Dictionary<int, Partition> Partitions { get; set; }

        public AddressPartitionSettings()
        {
            Partitions = new Dictionary<int, Partition>();
        }
    }
}
