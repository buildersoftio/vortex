
using Vortex.Core.Models.Common.Clusters;

namespace Vortex.Core.Models.Configurations
{
    public class ClusterConfiguration
    {
        public string Name { get; set; }
        public Dictionary<string, NodeConfig> Nodes { get; set; }

        public ClusterSettings Settings { get; set; }

        public ClusterConfiguration()
        {
            Nodes = new Dictionary<string, NodeConfig>();
            Settings = new ClusterSettings();
        }
    }

    public class NodeConfig
    {
        public string Address { get; set; }
        public int Port { get; set; }
    }
}
