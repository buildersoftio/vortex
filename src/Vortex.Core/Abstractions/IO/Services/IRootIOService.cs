namespace Vortex.Core.IO.Services
{
    public interface IRootIOService
    {
        bool IsDataRootDirectoryCreated();
        bool IsConfigRootDirectoryCreated();
        bool IsLogsRootDirectoryCreated();
        bool IsTempRootDirectoryCreated();


        bool CreateDataRootDirectory();

        bool CreateConfigRootDirectory();
        bool CreateLogsRootDirectory();
        bool CreateTempRootDirectory();

        bool IsInitialConfiguration();
    }
}
