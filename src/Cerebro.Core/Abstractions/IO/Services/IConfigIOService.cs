namespace Cerebro.Core.IO.Services
{
    public interface IConfigIOService
    {
        bool IsActiveDirectoryCreated();
        bool CreateActiveDirectory();
    }
}
