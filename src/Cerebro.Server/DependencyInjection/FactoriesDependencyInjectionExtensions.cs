using Cerebro.Core.Abstractions.Factories;
using Cerebro.Infrastructure.Factories;

namespace Cerebro.Server.DependencyInjection
{
    public static class FactoriesDependencyInjectionExtensions
    {
        public static void AddFactories(this IServiceCollection services)
        {
            services.AddSingleton<IPartitionDataFactory, PartitionDataFactory>();
        }
    }
}
