using Cerebro.Core.IO;
using Cerebro.Core.IO.Services;
using Microsoft.Extensions.Logging;

namespace Cerebro.Infrastructure.IO.Services
{
    public class DataIOService : IDataIOService
    {
        private readonly ILogger<DataIOService> _logger;

        public DataIOService(ILogger<DataIOService> logger)
        {
            _logger = logger;
        }

        public bool CreateAddressDir(int addressId)
        {
            try
            {
                Directory.CreateDirectory(DataLocations.GetAddressDirectory(addressId));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"'{DataLocations.GetAddressDirectory(addressId)}' is not created, error details:{ex.Message}");
                return false;
            }
        }

        public bool CreateAddressPartitionDir(int addressId, int partitionId)
        {
            try
            {
                Directory.CreateDirectory(DataLocations.GetAddressPartitionDirectory(addressId, partitionId));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"'{DataLocations.GetAddressPartitionDirectory(addressId, partitionId)}' is not created, error details:{ex.Message}");
                return false;
            }
        }

        public bool CreateDataRootAddressesDir()
        {
            try
            {
                Directory.CreateDirectory(DataLocations.GetStoreDirectory());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"'data/store' folder is not created, error details:{ex.Message}");
                return false;
            }
        }

        public bool CreateIndexesDirectory()
        {
            try
            {
                Directory.CreateDirectory(DataLocations.GetIndexesDirectory());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"'data/store/indexes' folder is not created, error details:{ex.Message}");
                return false;
            }
        }

        public bool IsAddressDirCreated(int addressId)
        {
            if (Directory.Exists(DataLocations.GetAddressDirectory(addressId)))
                return true;

            return false;
        }

        public bool IsAddressPartitionDirCreated(int addressId, int partitionId)
        {
            if (Directory.Exists(DataLocations.GetAddressPartitionDirectory(addressId, partitionId)))
                return true;

            return false;
        }

        public bool IsDataRootAddressesDirCreated()
        {
            if (Directory.Exists(DataLocations.GetStoreDirectory()))
                return true;

            return false;
        }

        public bool IsIndexesDirectoryCreated()
        {
            if (Directory.Exists(DataLocations.GetIndexesDirectory()))
                return true;

            return false;
        }
    }
}
