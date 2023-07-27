using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.Services.ServerStates;

namespace Cerebro.Server.DependencyInjection
{
    public static class ServiceDependencyInjectionExtensions
    {
        public static void AddServerStateServices(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationService, ApplicationService>();
            services.AddSingleton<IAddressService, AddressService>();
        }
    }
}
