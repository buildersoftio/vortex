namespace Cerebro.Core.Models.Common.Clusters
{
    public class ClusterSettings
    {
        public int HeartbeatInterval { get; set; }
        public int HeartbeatTimeout { get; set; }

        public ClusterSettings()
        {
            HeartbeatInterval = 10;
            HeartbeatTimeout = 30;
        }
    }
}
