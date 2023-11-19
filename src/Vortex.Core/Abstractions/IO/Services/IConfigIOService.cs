using Vortex.Core.Models.Configurations;

namespace Vortex.Core.IO.Services
{
    public interface IConfigIOService
    {
        bool IsActiveDirectoryCreated();
        bool CreateActiveDirectory();
        bool CreateStorageDefaultActiveFile();
        StorageDefaultConfiguration? GetStorageDefaultConfiguration();

        bool CreateClusterActiveFile();
        ClusterConfiguration? GetClusterConfiguration();
    }
}
