using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cerebro.Core.Models.Common.Clusters
{
    public class Node
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public NodeStatus? Status { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        //public NodeState? State { get; set; }

        public DateTime? LastHeartbeat { get; set; }

        public Node()
        {
            Status = NodeStatus.Offline;
            LastHeartbeat = DateTime.Now;
        }
    }
}
