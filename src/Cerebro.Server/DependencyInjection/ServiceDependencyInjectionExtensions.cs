using Cerebro.Core.Abstractions.Background;
using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Services.Background;
using Cerebro.Core.Services.Entries;
using Cerebro.Core.Services.ServerStates;

namespace Cerebro.Server.DependencyInjection
{
    public static class ServiceDependencyInjectionExtensions
    {
        public static void AddServerStateServices(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationService, ApplicationService>();
            services.AddSingleton<IAddressService, AddressService>();
            services.AddSingleton<IPartitionEntryService, PartitionEntryService>();
        }

        public static void AddBackgroundServerStateServices(this IServiceCollection services)
        {
            services.AddSingleton<IBackgroundServerStateService<Address>, AddressBackgroundServerStateService>();
        }
    }
}
