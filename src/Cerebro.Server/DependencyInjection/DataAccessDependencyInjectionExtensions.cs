
using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.Abstractions.Repositories;
using Cerebro.Core.Repositories;
using Cerebro.Core.Repositories.Clustering;
using Cerebro.Infrastructure.DataAccess.IndexesState;
using Cerebro.Infrastructure.DataAccess.ServerStateStore;
using Cerebro.Infrastructure.Repositories;

namespace Cerebro.Server.DependencyInjection
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
        }
    }
}
