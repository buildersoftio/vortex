using Cerebro.Core.Services;


namespace Cerebro.Infrastructure.DependencyInjection
{
    public static class SystemStarterDependencyInjectionExtensions
    {
        public static void AddSystemStarterService(this IServiceCollection services)
        {
            services.AddSingleton<SystemStarterService>();
        }

        public static void UseSystemStarterService(this IApplicationBuilder builder)
        {
            var appMain = builder.ApplicationServices.GetService<SystemStarterService>();
        }
    }
}
