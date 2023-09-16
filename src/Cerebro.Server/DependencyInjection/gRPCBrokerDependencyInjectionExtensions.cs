using Cerebro.Core.Abstractions.Clients;
using Cerebro.Grpc.Servers;

namespace Cerebro.Server.DependencyInjection
{
    public static class gRPCBrokerDependencyInjectionExtensions
    {
        public static void AddGRPCBrokerServer(this IServiceCollection services)
        {
            services.AddSingleton<IClientIntegrationServer, ClientServer>();
        }
    }
}
