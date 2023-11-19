using Cerebro.Core.Models.Common.Addresses;

namespace Cerebro.Core.Models.Configurations
{
    public class StorageDefaultConfiguration
    {
        public CompressionTypes DefaultMessageCompressionType { get; set; }
        public ulong DefaultKeepLogFileNumber { get; set; }
        public uint DefaultDumpStatsInSeconds { get; set; }
        public ulong DefaultMaxLogFileSizeInBytes { get; set; }

        public ulong DeleteObsoleteFilesPeriodMilliseconds { get; set; }
        public bool EnableWriteThreadAdaptiveYield { get; set; }
        public int MaxFileOpeningThreads { get; set; }


        // Flushing options
        // write_buffer_size sets the size of a single memtable. Once memtable exceeds this size, it is marked immutable and a new one is created, for now we are creating as 64MB SIZE
        public ulong DefaultWriteBufferSizeInBytes { get; set; }
        //max_write_buffer_number sets the maximum number of memtables, both active and immutable. If the active memtable fills up and the total number of memtables is larger than max_write_buffer_number we stall further writes. This may happen if the flush process is slower than the write rate.
        public int DefaultMaxWriteBufferNumber { get; set; }

        public int DefaultMaxWriteBufferSizeToMaintain { get; set; }

        //min_write_buffer_number_to_merge is the minimum number of memtables to be merged before flushing to storage. For example, if this option is set to 2, immutable memtables are only flushed when there are two of them - a single immutable memtable will never be flushed. If multiple memtables are merged together, less data may be written to storage since two updates are merged to a single key. However, every Get() must traverse all immutable memtables linearly to check if the key is there. Setting this option too high may hurt read performance.
        public int DefaultMinWriteBufferNumberToMerge { get; set; }

        public int DefaultMaxBackgroundCompactionsThreads { get; set; }
        public int DefaultMaxBackgroundFlushesThreads { get; set; }

        public StorageDefaultConfiguration()
        {
            DefaultMessageCompressionType = CompressionTypes.NONE;

            DeleteObsoleteFilesPeriodMilliseconds = 30 * 1000 * 60;
            EnableWriteThreadAdaptiveYield = true;
            MaxFileOpeningThreads = 4;


            DefaultKeepLogFileNumber = 5;
            DefaultMaxLogFileSizeInBytes = 104857600;
            DefaultDumpStatsInSeconds = 60;


            //64MB
            DefaultWriteBufferSizeInBytes = 64000000;
            DefaultMaxWriteBufferNumber = 4;
            DefaultMaxWriteBufferSizeToMaintain = 0;
            DefaultMinWriteBufferNumberToMerge = 2;

            DefaultMaxBackgroundCompactionsThreads = 1;
            DefaultMaxBackgroundFlushesThreads = 1;
        }

        public void UpdateStorageDefaultConfigs(StorageDefaultConfiguration storageDefaultConfiguration)
        {
            DefaultMessageCompressionType = storageDefaultConfiguration.DefaultMessageCompressionType;
            DefaultKeepLogFileNumber = storageDefaultConfiguration.DefaultKeepLogFileNumber;
            DefaultDumpStatsInSeconds = storageDefaultConfiguration.DefaultDumpStatsInSeconds;
            DefaultMaxLogFileSizeInBytes = storageDefaultConfiguration.DefaultMaxLogFileSizeInBytes;

            DeleteObsoleteFilesPeriodMilliseconds = storageDefaultConfiguration.DeleteObsoleteFilesPeriodMilliseconds;
            EnableWriteThreadAdaptiveYield = storageDefaultConfiguration.EnableWriteThreadAdaptiveYield;
            MaxFileOpeningThreads = storageDefaultConfiguration.MaxFileOpeningThreads;

            DefaultWriteBufferSizeInBytes = storageDefaultConfiguration.DefaultWriteBufferSizeInBytes;
            DefaultMaxWriteBufferNumber = storageDefaultConfiguration.DefaultMaxWriteBufferNumber;

            DefaultMaxWriteBufferSizeToMaintain = storageDefaultConfiguration.DefaultMaxWriteBufferSizeToMaintain;

            DefaultMaxWriteBufferSizeToMaintain = storageDefaultConfiguration.DefaultMaxWriteBufferSizeToMaintain;

            DefaultMinWriteBufferNumberToMerge = storageDefaultConfiguration.DefaultMinWriteBufferNumberToMerge;
            DefaultMaxBackgroundCompactionsThreads = storageDefaultConfiguration.DefaultMaxBackgroundCompactionsThreads;
            DefaultMaxBackgroundFlushesThreads = storageDefaultConfiguration.DefaultMaxBackgroundFlushesThreads;
        }
    }
}
