using Cerebro.Cluster.Infrastructure.Servers.gRPC;
using Cerebro.Core.Abstractions.Clustering;

namespace Cerebro.Server.DependencyInjection
{
    public static class gRPCClusterDependencyInjectionExtensions
    {
        public static void AddGRPCClusterServer(this IServiceCollection services)
        {
            services.AddSingleton<INodeExchangeServer, NodeExchangeServer>();
        }
    }
}
