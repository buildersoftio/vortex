using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Models.BackgroundTimerRequests;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Containers;

namespace Vortex.Core.Services.Routing.Background
{
    public class SubscriptionEntryBackgroundService : TimedBackgroundServiceBase<SubscriptionEntryTimerRequest>
    {
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly SubscriptionContainer _subscriptionContainer;
        private readonly ISubscriptionEntryService _subscriptionEntryService;

        private static TimeSpan GetPeriod(NodeConfiguration nodeConfiguration)
        {
            return new TimeSpan(0, 0, nodeConfiguration.IdleClientConnectionInterval);
        }

        public SubscriptionEntryBackgroundService(NodeConfiguration nodeConfiguration,
            SubscriptionContainer subscriptionContainer,
            ISubscriptionEntryService subscriptionEntryService) : base(GetPeriod(nodeConfiguration))
        {
            _nodeConfiguration = nodeConfiguration;
            _subscriptionContainer = subscriptionContainer;
            _subscriptionEntryService = subscriptionEntryService;

            // starting the service....
            // CHECK: if the service doesn't start here, move it to ServerCoreStateManager.
            //        at LoadApplicationSubscriptions
            base.Start();
        }

        public override void OnTimer_Callback(object? state)
        {
            foreach (var entry in _subscriptionContainer.SubscriptionEntries!)
            {
                // flushing subscription entries, for subscription in subscriptionContainer
                entry.UpdatedAt = DateTime.UtcNow;
                entry.UpdatedBy = "bg_subscription_service";

                _subscriptionEntryService.UpdateSubscriptionEntries(entry);
            }
        }
    }
}
