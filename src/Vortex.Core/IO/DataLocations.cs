using Vortex.Core.Models.Entities.Addresses;

namespace Vortex.Core.IO
{
    public static class DataLocations
    {
        public static string GetStoreDirectory()
        {
            return Path.Combine(RootLocations.GetDataRootDirectory(), "store");
        }

        public static string GetIndexesDirectory()
        {
            return Path.Combine(GetStoreDirectory(), "indexes");
        }

        public static string GetIndexStateFile()
        {
            return Path.Combine(GetIndexesDirectory(), "indexes_entries_catalog_store.cbs");
        }

        public static string GetAddressDirectory(int addressId)
        {
            return Path.Combine(GetStoreDirectory(), addressId.ToString());
        }

        public static string GetAddressPartitionDirectory(int addressId, int partitionId)
        {
            return Path.Combine(GetAddressDirectory(addressId), $"P-{partitionId}");
        }

        public static string GetSimpleAddressPartitionDirectory(int addressId, int partitionId)
        {
            return Path.Combine(GetAddressDirectory(addressId), $"P-{partitionId}");
        }
    }
}
