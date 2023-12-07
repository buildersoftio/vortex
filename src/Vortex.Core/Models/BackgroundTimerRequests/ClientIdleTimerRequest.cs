namespace Vortex.Core.Models.BackgroundTimerRequests
{
    public class ClientIdleTimerRequest
    {
        public Guid Id { get; set; }
        public ClientIdleTimerRequest()
        {
            Id = Guid.NewGuid();
        }
    }
}
