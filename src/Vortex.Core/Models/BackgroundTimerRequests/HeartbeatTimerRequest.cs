namespace Cerebro.Core.Models.BackgroundRequests
{
    public class HeartbeatTimerRequest
    {
        public Guid RequestId { get; set; }
        public HeartbeatTimerRequest()
        {
            RequestId = Guid.NewGuid();
        }
    }
}
