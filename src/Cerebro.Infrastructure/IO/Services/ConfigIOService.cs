using Cerebro.Core.IO;
using Cerebro.Core.IO.Services;
using Cerebro.Core.Models.Configurations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cerebro.Infrastructure.IO.Services
{
    public class ConfigIOService : IConfigIOService
    {
        private readonly ILogger<ConfigIOService> _logger;

        public ConfigIOService(ILogger<ConfigIOService> logger)
        {
            _logger = logger;
        }
        public bool CreateActiveDirectory()
        {
            try
            {
                Directory.CreateDirectory(ConfigLocations.GetActiveDirectory());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"config/active folder is not created, error details:{ex.Message}");
                return false;
            }
        }

        public bool CreateStorageDefaultActiveFile()
        {
            try
            {
                if (File.Exists(ConfigLocations.GetActiveDefaultStorageConfigurationFile()) != true)
                {
                    // Create new active file
                    File.Copy(ConfigLocations.GetDefaultStorageConfigurationFile(), ConfigLocations.GetActiveDefaultStorageConfigurationFile(), overwrite: true);
                    _logger.LogInformation("Initial storage configuration has been copied successfully");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"storage_initial.json configuration file is missing at {ConfigLocations.GetDefaultStorageConfigurationFile()} or cannot parse, error details:{ex.Message}");
                return false;
            }
        }

        public StorageDefaultConfiguration? GetStorageDefaultConfiguration()
        {
            try
            {
                var storage = new StorageDefaultConfiguration();
                storage = JsonConvert.DeserializeObject<StorageDefaultConfiguration>(File.ReadAllText(ConfigLocations.GetActiveDefaultStorageConfigurationFile()));
                _logger.LogInformation("Changing state of default storage configuration");

                return storage;
            }
            catch (Exception ex)
            {
                _logger.LogError($"storage_config.json configuration cannot parse, error details:{ex.Message}");
                return null;
            }
            
        }

        public bool IsActiveDirectoryCreated()
        {
            if (Directory.Exists(ConfigLocations.GetActiveDirectory()) == true)
                return true;

            return false;
        }
    }
}
