using Cerebro.Core.Models.Entities.Entries;

namespace Cerebro.Core.Abstractions.Repositories
{
    public interface IPartitionEntryRepository
    {
        bool AddPartitionEntry(PartitionEntry partitionEntry);
        bool UpdatePartitionEntry(PartitionEntry partitionEntry);

        PartitionEntry? GetPartitionEntry(int addressId, int partitionId);
        PartitionEntry? GetPartitionEntry(string addressAlias, int partitionId);
        List<PartitionEntry> GetPartitionEntries(int addressId);
        List<PartitionEntry> GetPartitionEntries(string addressAlias);
    }
}
