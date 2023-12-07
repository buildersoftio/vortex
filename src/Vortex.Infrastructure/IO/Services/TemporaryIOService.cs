using Vortex.Core.Abstractions.IO.Services;
using Vortex.Core.IO;
using Vortex.Core.Utilities.Json;
using Microsoft.Extensions.Logging;

namespace Vortex.Infrastructure.IO.Services
{
    public class TemporaryIOService : ITemporaryIOService
    {
        private readonly ILogger<TemporaryIOService> _logger;

        public TemporaryIOService(ILogger<TemporaryIOService> logger)
        {
            _logger = logger;
        }
        public bool IsTemporaryBackgroundDirectoryCreated()
        {
            if (Directory.Exists(TemporaryLocations.GetBackgroundServiceDirectory()) == true)
                return true;

            return false;
        }

        public bool CreateTemporaryBackgroundDirectory()
        {
            try
            {
                Directory.CreateDirectory(TemporaryLocations.GetBackgroundServiceDirectory());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Temporary background folder is not created, error details:{ex.Message}");
                return false;
            }
        }

        public bool DeleteFile(string location)
        {
            try
            {
                File.Delete(location);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string[] GetBackgroundFiles(string prefixFileName)
        {
            var files = Directory.GetFiles(TemporaryLocations.GetBackgroundServiceDirectory(), $"{prefixFileName}*", SearchOption.AllDirectories);

            // sort the files based on date of creation, order by the oldest.
            var orderedFiles = files.Select(file => new FileInfo(file))
                                .OrderBy(fileInfo => fileInfo.CreationTime)
                                .ToList().Select(f=>f.FullName).ToArray();

            return orderedFiles;
        }

        public TRequest GetBackgroundTemporaryFileContent<TRequest>(string path)
        {
            try
            {
                var file = File
                    .ReadAllText(path)
                    .JsonToObject<TRequest>();

                return file;
            }
            catch (Exception)
            {
                return default!;
            }
        }



        public bool StoreBackgroundTemporaryFile<TRequest>(TRequest request, string prefixFileName, Guid guid)
        {
            try
            {
                string requestJson = request!.ToPrettyJson();
                File.WriteAllText(TemporaryLocations.GetBackgroundServiceFailedFile(prefixFileName + guid), requestJson);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
