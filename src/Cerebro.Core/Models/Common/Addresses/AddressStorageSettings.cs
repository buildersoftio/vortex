namespace Cerebro.Core.Models.Common.Addresses
{
    public class AddressStorageSettings
    {

        public CompressionTypes MessageCompressionType { get; set; }
        public ulong WriteBufferSizeInBytes { get; set; }
        public int MaxWriteBufferNumber { get; set; }
        public int MaxWriteBufferSizeToMaintain { get; set; }
        public int MinWriteBufferNumberToMerge { get; set; }
        public int MaxBackgroundCompactionsThreads { get; set; }
        public int MaxBackgroundFlushesThreads { get; set; }

        public AddressStorageSettings()
        {
            MessageCompressionType = CompressionTypes.NONE;

            //64MB
            WriteBufferSizeInBytes = 64000000;
            MaxWriteBufferNumber = 4;
            MaxWriteBufferSizeToMaintain = 0;
            MinWriteBufferNumberToMerge = 2;

            MaxBackgroundCompactionsThreads = 1;
            MaxBackgroundFlushesThreads = 1;
        }
    }

    public enum CompressionTypes
    {
        NONE,
        ZSTD,
        LZ4
    }
}
