namespace Cerebro.Core.IO
{
    public static class DataLocations
    {
        public static string GetStoreDirectory()
        {
            return Path.Combine(RootLocations.GetDataRootDirectory(), "store");
        }

        public static string GetAddressDirectory(int addressId)
        {
            return Path.Combine(GetStoreDirectory(), addressId.ToString());
        }

        public static string GetAddressPartitionDirectory(int addressId, string partitionId)
        {
            return Path.Combine(GetAddressDirectory(addressId), $"P-{partitionId}");
        }
    }
}
