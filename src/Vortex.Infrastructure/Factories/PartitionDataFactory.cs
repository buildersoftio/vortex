using Vortex.Core.Abstractions.Factories;
using Vortex.Core.Abstractions.Repositories.Data;
using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Models.Entities.Entries;
using Vortex.Infrastructure.Repositories.RocksDb;

namespace Vortex.Infrastructure.Factories
{
    public class PartitionDataFactory : IPartitionDataFactory
    {
        public IPartitionDataRepository CreatePartitionDataRepository(Address address, PartitionEntry partitionEntry)
        {
            return new RocksDbPartitionDataRepository(address, partitionEntry);
        }
    }
}
