using Cerebro.Core.IO.Services;
using Cerebro.Core.Models.Configurations;
using Cerebro.Core.Utilities.Consts;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Cerebro.Core.Services
{
    public class SystemStarterService
    {
        private readonly ILogger<SystemStarterService> _logger;
        private readonly IRootIOService _rootIOService;
        private readonly IConfigIOService _configIOService;
        private readonly NodeConfiguration _nodeConfiguration;

        public SystemStarterService(ILogger<SystemStarterService> logger, 
            IRootIOService rootIOService,
            IConfigIOService configIOService, 
            NodeConfiguration nodeConfiguration)
        {
            _logger = logger;
            _rootIOService = rootIOService;
            _configIOService = configIOService;
            _nodeConfiguration = nodeConfiguration;

            Start();

        }

        public void Start()
        {
            var generalColor = Console.ForegroundColor;

            Console.WriteLine($"                   Starting {SystemProperties.Name}");
            Console.WriteLine("                   Set your information in motion.");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("  ###"); Console.ForegroundColor = generalColor; Console.WriteLine("      ###");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("    ###"); Console.ForegroundColor = generalColor; Console.Write("  ###");
            Console.WriteLine($"       {SystemProperties.ShortName} {SystemProperties.Version}. Developed with (love) by Buildersoft LLC.");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("      ####         "); Console.ForegroundColor = generalColor; Console.WriteLine("Licensed under the Apache License 2.0. See https://bit.ly/3DqVQbx");
            Console.ForegroundColor = ConsoleColor.Blue;
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

            //var clusterService = serviceProvider.GetService<IClusterService>();

            _logger.LogInformation($"{SystemProperties.ShortName} is ready");

        }

        private void CreateLoggingDirectory()
        {
            if (_rootIOService.IsLogsRootDirectoryCreated() != true)
            {
                _logger.LogInformation("Creating 'LOGS' root directory");
                _rootIOService.CreateLogsRootDirectory();
            }
        }

        private void ExposePorts()
        {
            var exposedUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")!.Split(';');
            foreach (var url in exposedUrls)
            {
                try
                {
                    var u = new Uri(url);
                    if (u.Scheme == "https")
                        Console.WriteLine($"                   Port exposed {u.Port} SSL");
                    else
                        Console.WriteLine($"                   Port exposed {u.Port}");
                }
                catch (Exception)
                {
                    if (url.StartsWith("https://"))
                        Console.WriteLine($"                   Port exposed {url.Split(':').Last()} SSL");
                    else
                        Console.WriteLine($"                   Port exposed {url.Split(':').Last()}");
                }
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
                _logger.LogInformation("'DATA' root directory created");
                _rootIOService.CreateDataRootDirectory();
            }

            if (_rootIOService.IsConfigRootDirectoryCreated() != true)
            {
                _logger.LogInformation("'CONFIG' root directory created");
                _rootIOService.CreateConfigRootDirectory();
            }

            if (_rootIOService.IsTempRootDirectoryCreated() != true)
            {
                _logger.LogInformation("'LOGS' root directory created");
                _rootIOService.CreateTempRootDirectory();
            }
        }

        private void CheckConfigDirectories()
        {
            if (_configIOService.IsActiveDirectoryCreated() != true)
            {
                _logger.LogInformation("'CONFIG/ACTIVE' directory created");
                _configIOService.CreateActiveDirectory();
            }
        }

        private void CheckInitialStartingUp()
        {
            if (_rootIOService.IsInitialConfiguration() == true)
            {
                _logger.LogInformation("Doing Initial configuration");
            }
        }
    }
}
