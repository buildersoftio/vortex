using Cerebro.Core.IO;
using Cerebro.Core.IO.Services;
using Microsoft.Extensions.Logging;

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

        public bool IsActiveDirectoryCreated()
        {
            if (Directory.Exists(ConfigLocations.GetActiveDirectory()) == true)
                return true;

            return false;
        }
    }
}
