using Vortex.Core.Abstractions.Clustering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Concurrent;

namespace Vortex.Core.Models.Common.Clusters
{
    public class Cluster
    {
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ClusterStatus? Status { get; set; }

        public ConcurrentDictionary<string, Node> Nodes { get; set; }
        public ConcurrentDictionary<string, INodeExchangeClient> NodeExchangeClients { get; set; }

        public Cluster()
        {
            Status = ClusterStatus.Offline;
            Nodes = new ConcurrentDictionary<string, Node>();
            NodeExchangeClients = new ConcurrentDictionary<string, INodeExchangeClient>();
        }
    }
}
