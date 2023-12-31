﻿using Vortex.Core.Models.Common.Addresses;
using Vortex.Core.Models.Entities.Entries;

namespace Vortex.Core.Abstractions.Services
{
    public interface IPartitionEntryService
    {
        bool CreatePartitionEntry(int addressId, string addressAlias, int partitionId, MessageIndexTypes messageIndexType, string createdBy);
        PartitionEntry? GetPartitionEntry(int addressId, int partitionId);
        List<PartitionEntry> GetPartitionEntries(int addressId);

        bool UpdatePartitionEntry(PartitionEntry entry);
        bool DeletePartitionEntries(int addressId);
    }
}
