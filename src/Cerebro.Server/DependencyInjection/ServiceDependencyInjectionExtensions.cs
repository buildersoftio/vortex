using Cerebro.Core.Abstractions.Background;
using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.Abstractions.Services.Orchestrations;
using Cerebro.Core.Models.BackgroundRequests;
using Cerebro.Core.Models.Dtos.Addresses;
using Cerebro.Core.Models.Dtos.Applications;
using Cerebro.Core.Services.Background;
using Cerebro.Core.Services.Clustering;
using Cerebro.Core.Services.Clustering.Background;
using Cerebro.Core.Services.Entries;
using Cerebro.Core.Services.Orchestrations;
using Cerebro.Core.Services.ServerStates;

namespace Cerebro.Server.DependencyInjection
{
    public static class ServiceDependencyInjectionExtensions
    {
        public static void AddServerStateServices(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationService, ApplicationService>();
            services.AddSingleton<IAddressService, AddressService>();
            services.AddSingleton<IPartitionEntryService, PartitionEntryService>();
            services.AddSingleton<IClientConnectionService, ClientConnectionService>();
        }

        public static void AddOrchestators(this IServiceCollection services)
        {
            services.AddSingleton<IServerCoreStateManager, ServerCoreStateManager>();
            services.AddSingleton<IClusterManager, ClusterManager>();
        }

        public static void AddBackgroundServerStateServices(this IServiceCollection services)
        {
            services.AddSingleton<ISimpleBackgroundQueueService<AddressBackgroundRequest>, AddressBackgroundServerStateService>();
            services.AddSingleton<IBackgroundQueueService<AddressClusterScopeRequest>, AddressClusterSyncBackgroundService>();

            services.AddSingleton<IBackgroundQueueService<ApplicationClusterScopeRequest>, ApplicationClusterSyncBackgroundService>();
        }

        public static void AddBackgroundTimerServerStateServices(this IServiceCollection services)
        {
            services.AddSingleton<ITimedBackgroundService<HeartbeatTimerRequest>, HeartbeatBackgroundService>();
        }
    }
}
