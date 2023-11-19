using Vortex.Core.Abstractions.Repositories.Data;
using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Models.Entities.Entries;

namespace Vortex.Core.Abstractions.Factories
{
    public interface IPartitionDataFactory
    {
        IPartitionDataRepository CreatePartitionDataRepository(Address address, PartitionEntry partitionEntry);
    }
}
