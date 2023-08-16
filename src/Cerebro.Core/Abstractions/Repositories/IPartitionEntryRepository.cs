using Cerebro.Core.Models.Entities.Entries;

namespace Cerebro.Core.Abstractions.Repositories
{
    public interface IPartitionEntryRepository
    {
        bool AddPartitionEntry(PartitionEntry partitionEntry);
        bool UpdatePartitionEntry(PartitionEntry partitionEntry);
        bool DeletePartitionEntry(PartitionEntry partitionEntry);

        PartitionEntry? GetPartitionEntry(int addressId, int partitionId);
        List<PartitionEntry> GetPartitionEntries(int addressId);
        List<PartitionEntry> GetPartitionEntries(string addressAlias);
    }
}
