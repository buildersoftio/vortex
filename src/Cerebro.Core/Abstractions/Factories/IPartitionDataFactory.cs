using Cerebro.Core.Abstractions.Repositories.Data;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Models.Entities.Entries;

namespace Cerebro.Core.Abstractions.Factories
{
    public interface IPartitionDataFactory
    {
        IPartitionDataRepository CreatePartitionDataRepository(Address address, PartitionEntry partitionEntry);
    }
}
