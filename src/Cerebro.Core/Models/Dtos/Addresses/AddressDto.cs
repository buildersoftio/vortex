namespace Cerebro.Core.Models.Dtos.Addresses
{
    public class AddressDto
    {
        public string Alias { get; set; }
        public string Name { get; set; } // name will be in rules like "root/something" or "something"...
        public int SchemaId { get; set; }


        public int PartitionNumber { get; set; }

        public ulong WriteBufferSizeInBytes { get; set; }
        public int MaxWriteBufferNumber { get; set; }
        public int MaxWriteBufferSizeToMaintain { get; set; }
        public int MinWriteBufferNumberToMerge { get; set; }
        public int MaxBackgroundCompactionsThreads { get; set; }
        public int MaxBackgroundFlushesThreads { get; set; }
    }
}
