using Cerebro.Core.Abstractions.Factories;
using Cerebro.Core.Abstractions.Repositories.Data;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Models.Entities.Entries;
using Cerebro.Infrastructure.Repositories.RocksDb;

namespace Cerebro.Infrastructure.Factories
{
    public class PartitionDataFactory : IPartitionDataFactory
    {
        public IPartitionDataRepository CreatePartitionDataRepository(Address address, PartitionEntry partitionEntry)
        {
            return new RocksDbPartitionDataRepository(address, partitionEntry);
        }
    }
}
