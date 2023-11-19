using Vortex.Core.Services;

namespace Vortex.Server.DependencyInjection
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
