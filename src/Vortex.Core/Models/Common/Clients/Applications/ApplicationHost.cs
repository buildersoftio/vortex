namespace Vortex.Core.Models.Common.Clients.Applications
{
    public class ApplicationHost
    {
        public bool IsConnected { get; set; }
        public DateTimeOffset FirstConnectionDate { get; set; }
        public DateTimeOffset LastConnectionDate { get; set; }
        public DateTimeOffset? LastHeartbeatDate { get; set; }
    }
}
