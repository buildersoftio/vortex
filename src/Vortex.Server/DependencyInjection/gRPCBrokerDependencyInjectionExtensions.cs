using Vortex.Core.Abstractions.Clients;
using Vortex.Grpc.Servers;

namespace Vortex.Server.DependencyInjection
{
    public static class gRPCBrokerDependencyInjectionExtensions
    {
        public static void AddGRPCBrokerServer(this IServiceCollection services)
        {
            services.AddSingleton<IClientIntegrationServer, ClientServer>();
        }
    }
}
