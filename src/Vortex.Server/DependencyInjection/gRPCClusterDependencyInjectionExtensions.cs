using Vortex.Cluster.Infrastructure.Servers.gRPC;
using Vortex.Core.Abstractions.Clustering;

namespace Vortex.Server.DependencyInjection
{
    public static class gRPCClusterDependencyInjectionExtensions
    {
        public static void AddGRPCClusterServer(this IServiceCollection services)
        {
            services.AddSingleton<INodeExchangeServer, NodeExchangeServer>();
        }
    }
}
