using Cerebro.Core.Abstractions.Services.Data;
using Cerebro.Core.Models.Data;
using Cerebro.Core.Models.Entities.Entries;
using System.Collections.Concurrent;

namespace Cerebro.Core.Models.Containers
{
    public class AddressContainer
    {
        public string? AddressAlias { get; set; }
        public string? AddressName { get; set; }
        public List<PartitionEntry>? PartitionEntries { get; set; }

        public ConcurrentDictionary<int, IPartitionDataService<Message>> PartitionDataServices { get; set; }

        public AddressContainer()
        {
            PartitionDataServices = new ConcurrentDictionary<int, IPartitionDataService<Message>>();
        }
    }
}
