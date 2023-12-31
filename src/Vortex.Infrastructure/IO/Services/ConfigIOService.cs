﻿using Vortex.Core.IO;
using Vortex.Core.IO.Services;
using Vortex.Core.Models.Configurations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Vortex.Infrastructure.IO.Services
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

        public bool CreateClusterActiveFile()
        {
            try
            {
                if (File.Exists(ConfigLocations.GetActiveClusterConfigurationFile()) != true)
                {
                    // Create new active file
                    File.Copy(ConfigLocations.GetClusterConfigurationFile(), ConfigLocations.GetActiveClusterConfigurationFile(), overwrite: true);
                    _logger.LogInformation("Cluster configuration has been copied successfully");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"clusters_initial_config.json configuration file is missing at {ConfigLocations.GetClusterConfigurationFile()} or cannot parse, error details:{ex.Message}");
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
                    _logger.LogInformation("Default Storage configuration has been copied successfully");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"storage_initial.json configuration file is missing at {ConfigLocations.GetDefaultStorageConfigurationFile()} or cannot parse, error details:{ex.Message}");
                return false;
            }
        }

        public ClusterConfiguration? GetClusterConfiguration()
        {
            try
            {
                var storage = new ClusterConfiguration();
                storage = JsonConvert.DeserializeObject<ClusterConfiguration>(File.ReadAllText(ConfigLocations.GetActiveClusterConfigurationFile()));

                return storage;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Trying to read from cluster settings from {ConfigLocations.GetClusterConfigurationFile()}");
                _logger.LogError($"clusters_config.json configuration cannot parse, error details:{ex.Message}");
            }

            try
            {
                var storage = new ClusterConfiguration();
                storage = JsonConvert.DeserializeObject<ClusterConfiguration>(File.ReadAllText(ConfigLocations.GetClusterConfigurationFile()));

                return storage;

            }
            catch (Exception ex)
            {

                _logger.LogError($"clusters_initial_config.json configuration cannot parse, error details:{ex.Message}");
                return null;
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
