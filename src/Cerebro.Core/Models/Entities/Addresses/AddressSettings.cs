using Cerebro.Core.Models.Entities.Base;

namespace Cerebro.Core.Models.Entities.Addresses
{
    public class AddressSettings
    {
        public int PartitionNumber { get; set; }
        public ulong WriteBufferSizeInBytes { get; set; }
        public int MaxWriteBufferNumber { get; set; }
        public int MaxWriteBufferSizeToMaintain { get; set; }
        public int MinWriteBufferNumberToMerge { get; set; }
        public int MaxBackgroundCompactionsThreads { get; set; }
        public int MaxBackgroundFlushesThreads { get; set; }
    }
}
