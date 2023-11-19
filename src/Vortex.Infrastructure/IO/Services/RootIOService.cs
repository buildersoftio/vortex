using Vortex.Core.IO;
using Vortex.Core.IO.Services;
using Microsoft.Extensions.Logging;

namespace Vortex.Infrastructure.IO.Services
{
    public class RootIOService : IRootIOService
    {
        private readonly ILogger<RootIOService> _logger;

        public RootIOService(ILogger<RootIOService> logger)
        {
            _logger = logger;
        }

        public bool IsConfigRootDirectoryCreated()
        {
            if (Directory.Exists(RootLocations.GetConfigRootDirectory()) == true)
                return true;

            return false;
        }

        public bool IsDataRootDirectoryCreated()
        {
            if (Directory.Exists(RootLocations.GetDataRootDirectory()) == true)
                return true;

            return false;
        }

        public bool IsLogsRootDirectoryCreated()
        {
            if (Directory.Exists(RootLocations.GetLogsRootDirectory()) == true)
                return true;

            return false;
        }

        public bool IsTempRootDirectoryCreated()
        {
            if (Directory.Exists(RootLocations.GetTempRootDirectory()) == true)
                return true;

            return false;
        }


        public bool CreateConfigRootDirectory()
        {
            try
            {
                Directory.CreateDirectory(RootLocations.GetConfigRootDirectory());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Main Config folder is not created, error details:{ex.Message}");
                return false;
            }
        }

        public bool CreateDataRootDirectory()
        {
            try
            {
                Directory.CreateDirectory(RootLocations.GetDataRootDirectory());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Main Data folder is not created, error details:{ex.Message}");
                return false;
            }
        }

        public bool CreateLogsRootDirectory()
        {
            try
            {
                Directory.CreateDirectory(RootLocations.GetLogsRootDirectory());

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Main Logs folder is not created, error details:{ex.Message}");
                return false;
            }
        }

        public bool CreateTempRootDirectory()
        {
            try
            {
                Directory.CreateDirectory(RootLocations.GetTempRootDirectory());

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Main Temp folder is not created, error details:{ex.Message}");
                return false;
            }
        }

        public bool IsInitialConfiguration()
        {
            if (Directory.Exists(RootLocations.GetLogsRootDirectory()) != true ||
                Directory.Exists(RootLocations.GetDataRootDirectory()) != true ||
                Directory.Exists(RootLocations.GetConfigRootDirectory()) != true ||
                Directory.Exists(RootLocations.GetTempRootDirectory()) != true)
                return true;

            return false;
        }
    }
}
