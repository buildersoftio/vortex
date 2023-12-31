﻿using Vortex.Core.Models.Common.Addresses;
using Vortex.Core.Models.Entities.Base;

namespace Vortex.Core.Models.Entities.Entries
{
    public class PartitionEntry : BaseEntity
    {
        public int Id { get; set; }

        public string NodeOwner { get; set; }

        public int AddressId { get; set; }
        public int PartitionId { get; set; }
        public string AddressAlias { get; set; }

        public MessageIndexTypes MessageIndexType { get; set; }

        // here is the main entry, entries can be indexed asynchronously
        public long CurrentEntry { get; set; }
        public long MarkDeleteEntryPosition { get; set; }

        public long ClusterTemporaryCurrentEntry { get; set; }
        public long ClusterTemporaryMarkDeleteEntryPosition { get; set; }

        // index key will be generated by the AddressIndex
        public Dictionary<string, IndexPosition> Positions { get; set; }

        public PartitionEntry()
        {
            Positions = new Dictionary<string, IndexPosition>();
        }
    }

    public class IndexPosition
    {
        public long StartEntryPosition { get; set; }
        public long? EndEntryPosition { get; set; }
    }
}
