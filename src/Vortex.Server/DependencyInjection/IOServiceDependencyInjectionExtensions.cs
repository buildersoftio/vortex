using Vortex.Core.Abstractions.IO.Services;
using Vortex.Core.IO;
using Vortex.Core.IO.Services;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Utilities.Consts;
using Vortex.Infrastructure.IO.Services;

namespace Vortex.Server.DependencyInjection
{
    public static class IOServiceDependencyInjectionExtensions
    {
        public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.BindNodeConfiguration(configuration);
            services.BindDefaultStorageConfiguration();
            services.BindCredentialsConfiguration(configuration);

        }

        private static void BindNodeConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var nodeConfiguration = new NodeConfiguration();

            nodeConfiguration.NodeId = configuration.GetValue<string>(EnvironmentConstants.NodeId)!;

            nodeConfiguration.IdleClientConnectionInterval = configuration.GetValue<int>(EnvironmentConstants.BackgroundIdleClientConnectionInterval)!;
            nodeConfiguration.IdleClientConnectionTimeout = configuration.GetValue<int>(EnvironmentConstants.BackgroundIdleClientConnectionTimeout)!;
            nodeConfiguration.CheckRetryCount = configuration.GetValue<int>(EnvironmentConstants.BackgroundCheckRetryCount)!;
            nodeConfiguration.BackgroundPositionEntry_FlushInterval = configuration.GetValue<int>(EnvironmentConstants.BackgroundPositionEntityFlushInterval)!;

            nodeConfiguration.DefaultAutoCommitEntry = configuration.GetValue<bool>(EnvironmentConstants.DefaultAutoCommitEntry)!;
            nodeConfiguration.DefaultAcknowledgmentType = Enum.Parse<AcknowledgmentTypes>(configuration.GetValue<string>(EnvironmentConstants.DefaultAcknowledgmentType)!);
            nodeConfiguration.DefaultReadInitialPosition = Enum.Parse<ReadInitialPositions>(configuration.GetValue<string>(EnvironmentConstants.DefaultReadInitialPosition)!);

            services.AddSingleton(nodeConfiguration);
        }

        private static void BindDefaultStorageConfiguration(this IServiceCollection services)
        {
            StorageDefaultConfiguration defaultStorageConfig = new StorageDefaultConfiguration();
            services.AddSingleton(defaultStorageConfig);
        }

        public static void AddIOServices(this IServiceCollection services)
        {
            services.AddSingleton<IRootIOService, RootIOService>();
            services.AddSingleton<IConfigIOService, ConfigIOService>();
            services.AddSingleton<IDataIOService, DataIOService>();
            services.AddSingleton<ITemporaryIOService, TemporaryIOService>();
        }

        private static void BindCredentialsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var credentialsConfiguration = new List<CredentialsConfiguration>();
            configuration.Bind("Credentials", credentialsConfiguration);
            services.AddSingleton(credentialsConfiguration);
        }
    }
}
