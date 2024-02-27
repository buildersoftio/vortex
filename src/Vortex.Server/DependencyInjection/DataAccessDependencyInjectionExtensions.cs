
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.Repositories;
using Vortex.Core.Repositories;
using Vortex.Core.Repositories.Clustering;
using Vortex.Infrastructure.DataAccess.IndexesState;
using Vortex.Infrastructure.DataAccess.ServerStateStore;
using Vortex.Infrastructure.Repositories;

namespace Vortex.Server.DependencyInjection
{
    public static class DataAccessDependencyInjectionExtensions
    {
        public static void AddServerStateStore(this IServiceCollection services)
        {
            services.AddSingleton<ServerStateStoreDbContext>();
            services.AddSingleton<IndexCatalogDbContext>();
        }

        public static void AddServerRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationRepository, ApplicationRepository>();
            services.AddSingleton<IAddressRepository, AddressRepository>();
            services.AddSingleton<IPartitionEntryRepository, PartitionEntryRepository>();

            services.AddSingleton<IClusterStateRepository, ClusterStateRepository>();
            services.AddSingleton<ISubscriptionEntryRepository, SubscriptionEntryRepository>();
        }
    }
}
