using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Abstractions.Services.Orchestrations;
using Vortex.Core.Models.BackgroundRequests;
using Vortex.Core.Models.Dtos.Addresses;
using Vortex.Core.Models.Dtos.Applications;
using Vortex.Core.Services.Background;
using Vortex.Core.Services.Clustering;
using Vortex.Core.Services.Clustering.Background;
using Vortex.Core.Services.Entries;
using Vortex.Core.Services.Orchestrations;
using Vortex.Core.Services.ServerStates;

namespace Vortex.Server.DependencyInjection
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
