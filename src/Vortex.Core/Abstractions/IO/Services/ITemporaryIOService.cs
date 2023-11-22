namespace Vortex.Core.Abstractions.IO.Services
{
    public interface ITemporaryIOService
    {
        bool IsTemporaryBackgroundDirectoryCreated();
        bool CreateTemporaryBackgroundDirectory();
        bool StoreBackgroundTemporaryFile<TRequest>(TRequest request, string prefixFileName, Guid guid);
        bool DeleteFile(string path);
        string[] GetBackgroundFiles(string prefixFileName);

        TRequest GetBackgroundTemporaryFileContent<TRequest>(string path);
    }
}
