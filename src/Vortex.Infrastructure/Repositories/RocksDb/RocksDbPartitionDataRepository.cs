using Vortex.Core.Abstractions.Repositories.Data;
using Vortex.Core.IO;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Models.Entities.Entries;
using RocksDbSharp;

namespace Vortex.Infrastructure.Repositories.RocksDb
{
    public class RocksDbPartitionDataRepository : IPartitionDataRepository
    {
        private readonly Address _address;
        private readonly PartitionEntry _partitionEntry;
        private readonly StorageDefaultConfiguration _storageDefaultConfiguration;

        private readonly DbOptions dbOptions;
        private readonly RocksDbSharp.RocksDb rocksDb;

        private readonly ColumnFamilyOptions columnFamilyOptions;
        private readonly string[] cfNames;
        private readonly ColumnFamilies columnFamilies;


        private readonly ColumnFamilyHandle cfDefault;
        private readonly ColumnFamilyHandle cfTemp;

        public RocksDbPartitionDataRepository(Address address, PartitionEntry partitionEntry)
        {
            _address = address;
            _partitionEntry = partitionEntry;

            dbOptions = new DbOptions()
              // figure this out how to name the files.
              //.SetDbLogDir(_logPath)
              .SetCreateIfMissing(true)
              .SetCreateMissingColumnFamilies(true)
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


            // TODO: add settings for column family
            columnFamilyOptions = new ColumnFamilyOptions();
            cfNames = new[] { "default", "cluster", "indexes" };

            var columnFamilies = new ColumnFamilies();
            foreach (var cfName in cfNames)
            {
                columnFamilies.Add(cfName, columnFamilyOptions);
            }



            rocksDb = RocksDbSharp
                .RocksDb
                .Open(dbOptions, partitionLocation, columnFamilies);

            cfDefault = rocksDb.GetColumnFamily(cfNames[0]);
            cfTemp = rocksDb.GetColumnFamily(cfNames[1]);
        }

        public void CloseConnection()
        {
            rocksDb.Dispose();
        }

        public void Delete(byte[] entryId)
        {
            rocksDb
                .Remove(entryId, cfDefault);
        }

        public byte[] Get(byte[] entryId)
        {
            return rocksDb
                .Get(entryId, cfDefault);
        }

        public void Put(byte[] entryId, byte[] entity)
        {
            rocksDb
                .Put(entryId, entity, cfDefault);
        }

        public void Put(ReadOnlySpan<byte> entryId, ReadOnlySpan<byte> message)
        {
            rocksDb
                .Put(entryId, message, cfDefault);
        }

        public void PutTemporary(byte[] entryId, byte[] message)
        {
            rocksDb
                .Put(entryId, message, cfTemp);
        }

        public void PutTemporary(ReadOnlySpan<byte> entryId, ReadOnlySpan<byte> message)
        {
            rocksDb
                .Put(entryId, message, cfTemp);
        }
    }
}
