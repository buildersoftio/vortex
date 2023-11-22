using Vortex.Cluster.Infrastructure.Factories;
using Vortex.Core.Abstractions.Factories;
using Vortex.Infrastructure.Factories;

namespace Vortex.Server.DependencyInjection
{
    public static class FactoriesDependencyInjectionExtensions
    {
        public static void AddFactories(this IServiceCollection services)
        {
            services.AddSingleton<IPartitionDataFactory, PartitionDataFactory>();
            services.AddSingleton<INodeExchangeClientFactory, NodeExchangeClientFactory>();
        }
    }
}
