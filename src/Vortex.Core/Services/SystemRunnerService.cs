﻿using Vortex.Core.Abstractions.Clients;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.IO.Services;
using Vortex.Core.IO.Services;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Utilities.Consts;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using Vortex.Core.Abstractions.Background;
using Vortex.Core.Models.BackgroundTimerRequests;

namespace Vortex.Core.Services
{
    public class SystemRunnerService
    {
        private readonly ILogger<SystemRunnerService> _logger;
        private readonly IRootIOService _rootIOService;
        private readonly IConfigIOService _configIOService;
        private readonly IDataIOService _dataIOService;
        private readonly ITemporaryIOService _temporaryIOService;
        private readonly NodeConfiguration _nodeConfiguration;
        private INodeExchangeServer? _nodeExchangeServer;
        private IClientIntegrationServer? _brokerIntegrationServer;

        // from here we are changing the default state of the storage configuration
        private readonly StorageDefaultConfiguration _storageDefaultConfiguration;
        private readonly IClusterManager _clusterManager;
        private readonly IServiceProvider _serviceProvider;

        public SystemRunnerService(
            ILogger<SystemRunnerService> logger,
            IRootIOService rootIOService,
            IConfigIOService configIOService,
            IDataIOService dataIOService,
            ITemporaryIOService temporaryIOService,
            NodeConfiguration nodeConfiguration,
            StorageDefaultConfiguration storageDefaultConfiguration,
            IClusterManager clusterManager,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rootIOService = rootIOService;
            _configIOService = configIOService;
            _dataIOService = dataIOService;
            _temporaryIOService = temporaryIOService;
            _nodeConfiguration = nodeConfiguration;
            _storageDefaultConfiguration = storageDefaultConfiguration;
            _clusterManager = clusterManager;

            _serviceProvider = serviceProvider;

            Start();
        }

        public void Start()
        {

            Console.WriteLine(
                "\n                     _            " + $"       Starting {SystemProperties.Name}" +
                "\n    __   _____  _ __| |_ _____  __" + "       Set your information in motion." +
                "\n    \\ \\ / / _ \\| '__| __/ _ \\ \\/ /" +
                "\n     \\ V / (_) | |  | ||  __/>  < " + $"       {SystemProperties.ShortName} {SystemProperties.Version}. Developed with love by Buildersoft LLC." +
                "\n      \\_/ \\___/|_|   \\__\\___/_/\\_\\" + $"       Licensed under the Apache License 2.0. See https://bit.ly/3DqVQbx \n\n");

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
            SetDefaultEnvironmentVariables();
            Console.WriteLine("");

            _logger.LogInformation("Update settings");
            _logger.LogInformation($"Node identifier is '{_nodeConfiguration.NodeId}'");

            CheckRootDirectories();
            CheckConfigDirectories();

            UpdateStateOfDefaultConfiguration();

            RunCluster();
            RunBroker();
            RunBackgroundJobs();

            _logger.LogInformation($"{SystemProperties.ShortName} is ready");
        }

        private void SetDefaultEnvironmentVariables()
        {
            if (Environment.GetEnvironmentVariable(EnvironmentConstants.BackgroundServiceFailTaskInterval) == null)
            {
                _logger.LogInformation($"Environment variable:{EnvironmentConstants.BackgroundServiceFailTaskInterval}: 300");
                Environment.SetEnvironmentVariable(EnvironmentConstants.BackgroundServiceFailTaskInterval, "300");
            }

            _logger.LogInformation($"Environment variable:{EnvironmentConstants.BackgroundIdleClientConnectionInterval}: {_nodeConfiguration.IdleClientConnectionInterval}");
            _logger.LogInformation($"Environment variable:{EnvironmentConstants.BackgroundIdleClientConnectionTimeout}: {_nodeConfiguration.IdleClientConnectionTimeout}");
            _logger.LogInformation($"Environment variable:{EnvironmentConstants.BackgroundCheckRetryCount}: {_nodeConfiguration.CheckRetryCount}");
            _logger.LogInformation($"Environment variable:{EnvironmentConstants.BackgroundPositionEntityFlushInterval}: {_nodeConfiguration.BackgroundPositionEntry_FlushInterval}");
            _logger.LogInformation($"Environment variable:{EnvironmentConstants.BackgroundSubscriptionEntityFlushInterval}: {_nodeConfiguration.BackgroundSubscriptionEntry_FlushInternal}");
            _logger.LogInformation($"Environment variable:{EnvironmentConstants.DefaultAutoCommitEntry}: {_nodeConfiguration.DefaultAutoCommitEntry}");
            _logger.LogInformation($"Environment variable:{EnvironmentConstants.DefaultAcknowledgmentType}: {_nodeConfiguration.DefaultAcknowledgmentType}");
            _logger.LogInformation($"Environment variable:{EnvironmentConstants.DefaultReadInitialPosition}: {_nodeConfiguration.DefaultReadInitialPosition}");
        }

        private void CreateLoggingDirectory()
        {
            if (_rootIOService.IsLogsRootDirectoryCreated() != true)
            {
                _logger.LogInformation("Root directory [/logs] is created");
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
                            Console.WriteLine($"                  HTTPS  Port exposed {u.Port} SSL");
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
                Console.WriteLine($"                   Vortex is running on IIS Server");
            }

            if (Environment.GetEnvironmentVariable(EnvironmentConstants.VortexClusterConnectionPort) != null)
                Console.WriteLine($"                CLUSTER  Port exposed {Environment.GetEnvironmentVariable(EnvironmentConstants.VortexClusterConnectionPort)}");
            if (Environment.GetEnvironmentVariable(EnvironmentConstants.VortexClusterConnectionSSLPort) != null)
                Console.WriteLine($"                CLUSTER  Port exposed {Environment.GetEnvironmentVariable(EnvironmentConstants.VortexClusterConnectionSSLPort)} SSL");

            if (Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerPort) != null)
                Console.WriteLine($"                 BROKER  Port exposed {Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerPort)}");
            if (Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerConnectionSSLPort) != null)
                Console.WriteLine($"                 BROKER  Port exposed {Environment.GetEnvironmentVariable(EnvironmentConstants.BrokerConnectionSSLPort)} SSL");
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
                _logger.LogInformation("Root directory [/data] is created");
                _rootIOService.CreateDataRootDirectory();
            }

            // create data/store directory
            if (_dataIOService.IsDataRootAddressesDirCreated() != true)
            {
                _logger.LogInformation("Root directory [/data/store] is created");
                _dataIOService.CreateDataRootAddressesDir();
            }

            if (_dataIOService.IsIndexesDirectoryCreated() != true)
            {
                _logger.LogInformation("Root directory [/data/store/indexes] is created");
                _dataIOService.CreateIndexesDirectory();
            }


            if (_rootIOService.IsConfigRootDirectoryCreated() != true)
            {
                _logger.LogInformation("Root directory [/config] is created");
                _rootIOService.CreateConfigRootDirectory();
            }

            if (_rootIOService.IsTempRootDirectoryCreated() != true)
            {
                _logger.LogInformation("Root directory [/logs] is created");
                _rootIOService.CreateTempRootDirectory();
            }

            if (_temporaryIOService.IsTemporaryBackgroundDirectoryCreated() != true)
            {
                _logger.LogInformation("Root directory [/logs/backgrounds] is created");
                _temporaryIOService.CreateTemporaryBackgroundDirectory();
            }
        }

        private void CheckConfigDirectories()
        {
            if (_configIOService.IsActiveDirectoryCreated() != true)
            {
                _logger.LogInformation("Directory [/config/active] is created");
                _configIOService.CreateActiveDirectory();

                _configIOService.CreateStorageDefaultActiveFile();

                _configIOService.CreateClusterActiveFile();
            }
        }

        private void UpdateStateOfDefaultConfiguration()
        {
            // updating the default storage configuration
            _storageDefaultConfiguration.UpdateStorageDefaultConfigs(_configIOService.GetStorageDefaultConfiguration()!);
        }

        private void CheckInitialStartingUp()
        {
            if (_rootIOService.IsInitialConfiguration() == true)
            {
                _logger.LogInformation("Doing initial configuration");
            }
        }


        private void RunCluster()
        {
            _nodeExchangeServer = _serviceProvider.GetService(typeof(INodeExchangeServer)) as INodeExchangeServer;
            _nodeExchangeServer!.Start();
            _clusterManager.Start();
        }


        private void RunBroker()
        {
            _brokerIntegrationServer = _serviceProvider.GetService(typeof(IClientIntegrationServer)) as IClientIntegrationServer;
            _brokerIntegrationServer!.Start();
        }

        private void RunBackgroundJobs()
        {
            var clientIdleBackgroundService = _serviceProvider.GetService(typeof(ITimedBackgroundService<ClientIdleTimerRequest>)) as ITimedBackgroundService<ClientIdleTimerRequest>;
            clientIdleBackgroundService!.Start();
        }
    }
}
