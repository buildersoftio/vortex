using Cerebro.Core.Models.Configurations;

namespace Cerebro.Core.Models.Common.Addresses
{
    public class AddressStorageSettings
    {

        public CompressionTypes MessageCompressionType { get; set; }

        public ulong KeepLogFileNumber { get; set; }
        public uint DumpStatsInSeconds { get; set; }
        public ulong MaxLogFileSizeInBytes { get; set; }

        public ulong DeleteObsoleteFilesPeriodMilliseconds { get; set; }
        public bool EnableWriteThreadAdaptiveYield { get; set; }
        public int MaxFileOpeningThreads { get; set; }


        public ulong WriteBufferSizeInBytes { get; set; }
        public int MaxWriteBufferNumber { get; set; }
        public int MaxWriteBufferSizeToMaintain { get; set; }
        public int MinWriteBufferNumberToMerge { get; set; }
        public int MaxBackgroundCompactionsThreads { get; set; }
        public int MaxBackgroundFlushesThreads { get; set; }

        public AddressStorageSettings()
        {
            MessageCompressionType = CompressionTypes.NONE;

            DeleteObsoleteFilesPeriodMilliseconds = 30 * 1000 * 60;
            EnableWriteThreadAdaptiveYield = true;
            MaxFileOpeningThreads = 4;

            KeepLogFileNumber = 5;
            MaxLogFileSizeInBytes = 104857600;
            DumpStatsInSeconds = 60;

            //64MB
            WriteBufferSizeInBytes = 64000000;
            MaxWriteBufferNumber = 4;
            MaxWriteBufferSizeToMaintain = 0;
            MinWriteBufferNumberToMerge = 2;

            MaxBackgroundCompactionsThreads = 1;
            MaxBackgroundFlushesThreads = 1;
        }

        public AddressStorageSettings(StorageDefaultConfiguration storageDefaultConfiguration)
        {
            MessageCompressionType = storageDefaultConfiguration.DefaultMessageCompressionType;

            EnableWriteThreadAdaptiveYield = storageDefaultConfiguration.EnableWriteThreadAdaptiveYield;
            MaxFileOpeningThreads = storageDefaultConfiguration.MaxFileOpeningThreads;

            KeepLogFileNumber = storageDefaultConfiguration.DefaultKeepLogFileNumber;
            MaxLogFileSizeInBytes = storageDefaultConfiguration.DefaultMaxLogFileSizeInBytes;
            DumpStatsInSeconds = storageDefaultConfiguration.DefaultDumpStatsInSeconds;

            //64MB
            WriteBufferSizeInBytes = storageDefaultConfiguration.DefaultWriteBufferSizeInBytes;
            MaxWriteBufferNumber = storageDefaultConfiguration.DefaultMaxWriteBufferNumber;
            MaxWriteBufferSizeToMaintain = storageDefaultConfiguration.DefaultMaxWriteBufferSizeToMaintain;
            MinWriteBufferNumberToMerge = storageDefaultConfiguration.DefaultMinWriteBufferNumberToMerge;

            MaxBackgroundCompactionsThreads = storageDefaultConfiguration.DefaultMaxWriteBufferSizeToMaintain;
            MaxBackgroundFlushesThreads = storageDefaultConfiguration.DefaultMaxBackgroundFlushesThreads;
        }
    }

    public enum CompressionTypes
    {
        NONE,
        ZSTD,
        LZ4
    }
}
