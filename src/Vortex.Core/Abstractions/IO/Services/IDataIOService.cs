namespace Vortex.Core.IO.Services
{
    public interface IDataIOService
    {
        bool IsDataRootAddressesDirCreated();
        bool CreateDataRootAddressesDir();

        bool IsIndexesDirectoryCreated();
        bool CreateIndexesDirectory();

        bool IsAddressDirCreated(int addressId);
        bool CreateAddressDir(int addressId);

        bool IsAddressPartitionDirCreated(int addressId, int partitionId);
        bool CreateAddressPartitionDir(int addressId, int partitionId);
    }
}
