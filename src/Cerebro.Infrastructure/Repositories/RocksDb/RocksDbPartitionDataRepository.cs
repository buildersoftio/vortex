using Cerebro.Core.Abstractions.Repositories.Data;
using Cerebro.Core.IO;
using Cerebro.Core.Models.Configurations;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Models.Entities.Entries;
using RocksDbSharp;

namespace Cerebro.Infrastructure.Repositories.RocksDb
{
    public class RocksDbPartitionDataRepository : IPartitionDataRepository
    {
        private readonly Address _address;
        private readonly PartitionEntry _partitionEntry;
        private readonly StorageDefaultConfiguration _storageDefaultConfiguration;

        private readonly OptionsHandle dbOptions;
        private readonly RocksDbSharp.RocksDb rocksDb;

        public RocksDbPartitionDataRepository(Address address, PartitionEntry partitionEntry)
        {
            _address = address;
            _partitionEntry = partitionEntry;

            dbOptions = new DbOptions()
              // figure this out how to name the files.
              //.SetDbLogDir(_logPath)
              .SetCreateIfMissing(true)
              .SetMaxLogFileSize(address.Settings.StorageSettings.MaxLogFileSizeInBytes)
              .SetStatsDumpPeriodSec(address.Settings.StorageSettings.DumpStatsInSeconds)
              .SetKeepLogFileNum(address.Settings.StorageSettings.KeepLogFileNumber)
              .EnableStatistics()
              .SetMaxBackgroundCompactions(address.Settings.StorageSettings.MaxBackgroundCompactionsThreads)
              .SetMaxBackgroundFlushes(address.Settings.StorageSettings.MaxBackgroundFlushesThreads)
              .SetWriteBufferSize(address.Settings.StorageSettings.WriteBufferSizeInBytes)
              .SetMaxWriteBufferNumber(address.Settings.StorageSettings.MaxWriteBufferNumber)
              .SetMaxWriteBufferNumberToMaintain(address.Settings.StorageSettings.MaxWriteBufferSizeToMaintain)
              .SetMinWriteBufferNumberToMerge(address.Settings.StorageSettings.MinWriteBufferNumberToMerge);

            string partitionLocation = DataLocations.GetAddressPartitionDirectory(partitionEntry.AddressId, partitionEntry.PartitionId);

            rocksDb = RocksDbSharp
                .RocksDb
                .Open(dbOptions, partitionLocation);
        }

        public void Delete(byte[] entryId)
        {
            rocksDb
                .Remove(entryId);
        }

        public byte[] Get(byte[] entryId)
        {
            return rocksDb
                .Get(entryId);
        }

        public void Put(byte[] entryId, byte[] entity)
        {
            rocksDb
                .Put(entryId, entity);
        }
    }
}
