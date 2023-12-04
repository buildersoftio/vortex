namespace Vortex.Core.Models.Configurations
{
    public class NodeConfiguration
    {
        public string NodeId { get; set; }

        public int IdleClientConnectionInterval { get; set; }
        public int IdleClientConnectionTimeout { get; set; }
        //VORTEX_CHECK_RETRY_COUNT
        public int CheckRetryCount { get; set; }
    }
}
