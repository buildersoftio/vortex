using Vortex.Core.Models.Data;
using Vortex.Core.Models.Entities.Entries;
using System.Collections.Concurrent;
using Vortex.Core.Abstractions.Background;

namespace Vortex.Core.Models.Containers
{
    public class AddressContainer
    {
        public string? AddressAlias { get; set; }
        public string? AddressName { get; set; }
        public List<PartitionEntry>? PartitionEntries { get; set; }

        public ConcurrentDictionary<int, ParallelBackgroundQueueServiceBase<PartitionMessage>> PartitionDataProcessors { get; set; }


        // in case of round-robin data distribution when MessageId is null; we are using an variable state here to know which is the next partition;
        public int CurrentPartitionId { get; set; }

        public AddressContainer()
        {
            PartitionDataProcessors = new ConcurrentDictionary<int, ParallelBackgroundQueueServiceBase<PartitionMessage>>();

            CurrentPartitionId = -1;
        }
    }
}
