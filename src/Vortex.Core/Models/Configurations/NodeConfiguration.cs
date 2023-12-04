namespace Vortex.Core.Models.Configurations
{
    public class NodeConfiguration
    {
        public string NodeId { get; set; }

        public int IdleClientConnectionInterval { get; set; }
        public int IdleClientConnectionTimeout { get; set; }
    }
}
