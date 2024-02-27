using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Entities.Base;

namespace Vortex.Core.Models.Entities.Entries
{
    public class SubscriptionEntry : BaseEntity
    {
        public int Id { get; set; }

        // this should not exists here, NodeOwner is for PartitionEntry, in case Application is SingleScope, update should not be done in the cluster level
        // only if the Application scope is ClusterScope - the update state should be done at all nodes.
        // public string NodeOwner { get; set; }

        public string SubscriptionName { get; set; }

        public string ApplicationName { get; set; }
        public int ApplicationId { get; set; }

        public int AddressId { get; set; }
        public int PartitionId { get; set; }
        public string AddressAlias { get; set; }

        //committed offsets,
        public long ReadCommittedEntry { get; set; }

        public ConsumptionSettings ConsumptionSettings { get; set; }
    }
}
