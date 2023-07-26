using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.Services.States;

namespace Cerebro.Server.DependencyInjection
{
    public static class ServiceDependencyInjectionExtensions
    {
        public static void AddServerStateServices(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationService, ApplicationService>();
        }
    }
}
