using Cerebro.Core.IO;
using Cerebro.Core.IO.Services;
using Cerebro.Core.Models.Configurations;
using Cerebro.Infrastructure.IO.Services;

namespace Cerebro.Server.DependencyInjection
{
    public static class IOServiceDependencyInjectionExtensions
    {
        public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.BindNodeConfiguration(configuration);
            services.BindDefaultStorageConfiguration();
        }

        private static void BindNodeConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var nodeConfiguration = new NodeConfiguration();

            nodeConfiguration.NodeId = configuration.GetValue<string>("NodeId")!;

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
        }
    }
}
