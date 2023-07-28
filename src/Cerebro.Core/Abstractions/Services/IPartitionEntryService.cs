using Cerebro.Core.Models.Common.Addresses;

namespace Cerebro.Core.Abstractions.Services
{
    public interface IPartitionEntryService
    {
        bool CreatePartitionEntry(int addressId, string addressAlias, int partitionId, MessageIndexTypes messageIndexType, string createdBy);
    }
}
