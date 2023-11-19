using Cerebro.Core.Services;

namespace Cerebro.Server.DependencyInjection
{
    public static class SystemRunnerDependencyInjectionExtensions
    {
        public static void AddSystemStarterService(this IServiceCollection services)
        {
            services.AddSingleton<SystemRunnerService>();
        }

        public static void UseSystemStarterService(this IApplicationBuilder builder)
        {
            var appMain = builder.ApplicationServices.GetService<SystemRunnerService>();
        }
    }
}
