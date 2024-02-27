namespace Vortex.Core.Models.BackgroundTimerRequests
{
    public class SubscriptionEntryTimerRequest
    {
        public Guid Id { get; set; }
        public SubscriptionEntryTimerRequest()
        {
            Id = Guid.NewGuid();
        }
    }
}
