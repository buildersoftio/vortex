using Cerebro.Core.Models.Configurations;

namespace Cerebro.Core.IO.Services
{
    public interface IConfigIOService
    {
        bool IsActiveDirectoryCreated();
        bool CreateActiveDirectory();
        bool CreateStorageDefaultActiveFile();

        StorageDefaultConfiguration? GetStorageDefaultConfiguration();
    }
}
