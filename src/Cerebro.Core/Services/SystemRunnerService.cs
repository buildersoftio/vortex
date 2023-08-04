using Cerebro.Core.IO.Services;
using Cerebro.Core.Models.Configurations;
using Cerebro.Core.Utilities.Consts;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Cerebro.Core.Services
{
    public class SystemRunnerService
    {
        private readonly ILogger<SystemRunnerService> _logger;
        private readonly IRootIOService _rootIOService;
        private readonly IConfigIOService _configIOService;
        private readonly IDataIOService _dataIOService;
        private readonly NodeConfiguration _nodeConfiguration;

        public SystemRunnerService(ILogger<SystemRunnerService> logger,
            IRootIOService rootIOService,
            IConfigIOService configIOService,
            IDataIOService dataIOService,
            NodeConfiguration nodeConfiguration)
        {
            _logger = logger;
            _rootIOService = rootIOService;
            _configIOService = configIOService;
            _dataIOService = dataIOService;
            _nodeConfiguration = nodeConfiguration;

            Start();
        }

        public void Start()
        {
            var generalColor = Console.ForegroundColor;

            Console.WriteLine($"                   Starting {SystemProperties.Name}");
            Console.WriteLine("                   Set your information in motion.");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  ###"); Console.ForegroundColor = generalColor; Console.WriteLine("      ###");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("    ###"); Console.ForegroundColor = generalColor; Console.Write("  ###");
            Console.WriteLine($"       {SystemProperties.ShortName} {SystemProperties.Version}. Developed with love by Buildersoft LLC.");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("      ####         "); Console.ForegroundColor = generalColor; Console.WriteLine("Licensed under the Apache License 2.0. See https://bit.ly/3DqVQbx");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("    ###  ###");
            Console.Write("  ###      ###     "); Console.ForegroundColor = generalColor; Console.WriteLine("Cerebro is an open-source distributed streaming platform designed to deliver the best performance possible for high-performance data pipelines, streaming analytics, streaming between microservices and data integrations.");
            Console.WriteLine("");


            ExposePorts();

            Console.WriteLine("");
            Console.WriteLine($"                   Starting {SystemProperties.Name}...");
            Console.WriteLine("\n");

            CheckInitialStartingUp();
            CreateLoggingDirectory();

            _logger.LogInformation($"Starting {SystemProperties.Name}...");
            Console.WriteLine("");
            _logger.LogInformation($"Server environment:os.name: {GetOSName()}");
            _logger.LogInformation($"Server environment:os.platform: {Environment.OSVersion.Platform}");
            _logger.LogInformation($"Server environment:os.version: {Environment.OSVersion}");
            _logger.LogInformation($"Server environment:os.is64bit: {Environment.Is64BitOperatingSystem}");
            _logger.LogInformation($"Server environment:domain.user.name: {Environment.UserDomainName}");
            _logger.LogInformation($"Server environment:user.name: {Environment.UserName}");
            _logger.LogInformation($"Server environment:processor.count: {Environment.ProcessorCount}");
            _logger.LogInformation($"Server environment:dotnet.version: {Environment.Version}");
            Console.WriteLine("");

            _logger.LogInformation("Update settings");
            _logger.LogInformation($"Node identifier is '{_nodeConfiguration.NodeId}'");

            CheckRootDirectories();
            CheckConfigDirectories();

            _logger.LogInformation($"{SystemProperties.ShortName} is ready");

        }

        private void CreateLoggingDirectory()
        {
            if (_rootIOService.IsLogsRootDirectoryCreated() != true)
            {
                _logger.LogInformation("'logs' root directory is created");
                _rootIOService.CreateLogsRootDirectory();
            }
        }

        private void ExposePorts()
        {
            try
            {
                var exposedUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")!.Split(';');
                foreach (var url in exposedUrls)
                {
                    try
                    {
                        var u = new Uri(url);
                        if (u.Scheme == "https")
                            Console.WriteLine($"                   HTTPS Port exposed {u.Port} SSL");
                        else
                            Console.WriteLine($"                   HTTP  Port exposed {u.Port}");
                    }
                    catch (Exception)
                    {
                        if (url.StartsWith("https://"))
                            Console.WriteLine($"                   HTTPS Port exposed {url.Split(':').Last()} SSL");
                        else
                            Console.WriteLine($"                   HTTP  Port exposed {url.Split(':').Last()}");
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"                   Cerebro is running in IIS Server");
            }

        }
        private string GetOSName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
            else if (RuntimeInformation.IsOSPlatform(osPlatform: OSPlatform.OSX))
            {
                return "MacOS";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            else
            {
                return "NOT_SUPPORTED";
            }
        }

        private void CheckRootDirectories()
        {
            if (_rootIOService.IsDataRootDirectoryCreated() != true)
            {
                _logger.LogInformation("'data' root directory is created");
                _rootIOService.CreateDataRootDirectory();
            }

            // create data/store directory
            if (_dataIOService.IsDataRootAddressesDirCreated() != true)
            {
                _logger.LogInformation("'data/store' root directory is created");
                _dataIOService.CreateDataRootAddressesDir();
            }

            if (_dataIOService.IsIndexesDirectoryCreated() != true)
            {
                _logger.LogInformation("'data/store/indexes' root directory is created");
                _dataIOService.CreateIndexesDirectory();
            }


            if (_rootIOService.IsConfigRootDirectoryCreated() != true)
            {
                _logger.LogInformation("'config' root directory is created");
                _rootIOService.CreateConfigRootDirectory();
            }

            if (_rootIOService.IsTempRootDirectoryCreated() != true)
            {
                _logger.LogInformation("'logs' root directory is created");
                _rootIOService.CreateTempRootDirectory();
            }
        }

        private void CheckConfigDirectories()
        {
            if (_configIOService.IsActiveDirectoryCreated() != true)
            {
                _logger.LogInformation("'config/active' directory is created");
                _configIOService.CreateActiveDirectory();
            }
        }

        private void CheckInitialStartingUp()
        {
            if (_rootIOService.IsInitialConfiguration() == true)
            {
                _logger.LogInformation("Doing initial configuration");
            }
        }
    }
}
