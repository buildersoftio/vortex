using Vortex.Core.Abstractions.Repositories;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Models.Common.Addresses;
using Vortex.Core.Models.Entities.Entries;
using Vortex.Core.Utilities.Extensions;
using Microsoft.Extensions.Logging;

namespace Vortex.Core.Services.Entries
{
    public class PartitionEntryService : IPartitionEntryService
    {
        private readonly ILogger<PartitionEntryService> _logger;
        private readonly IPartitionEntryRepository _partitionEntryRepository;

        public PartitionEntryService(ILogger<PartitionEntryService> logger, IPartitionEntryRepository partitionEntryRepository)
        {
            _logger = logger;
            _partitionEntryRepository = partitionEntryRepository;
        }

        public bool CreatePartitionEntry(int addressId, string addressAlias, int partitionId, MessageIndexTypes messageIndexType, string createdBy)
        {
            var partitionEntry = _partitionEntryRepository.GetPartitionEntry(addressId, partitionId);
            if (partitionEntry != null)
                return false;

            partitionEntry = new PartitionEntry()
            {
                AddressAlias = addressAlias,
                AddressId = addressId,
                PartitionId = partitionId,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = createdBy,
                MessageIndexType = messageIndexType,
                NodeOwner = "na",

                // Last update 11/12/2023 updated CurrentEntry from 1 to 0.
                CurrentEntry = 0,
                MarkDeleteEntryPosition = 0,
                Positions = new Dictionary<string, IndexPosition>()
                {
                    { DateTime.Now.GenerateAddressIndex(messageIndexType), new IndexPosition() { StartEntryPosition = 0 } }
                }
            };

            return _partitionEntryRepository.AddPartitionEntry(partitionEntry);
        }

        public bool DeletePartitionEntries(int addressId)
        {
            var partitionEntries = _partitionEntryRepository.GetPartitionEntries(addressId);

            foreach (var partition in partitionEntries)
            {
                if (_partitionEntryRepository.DeletePartitionEntry(partition) != true)
                    return false;
            }
            return true;
        }

        public List<PartitionEntry> GetPartitionEntries(int addressId)
        {
            return _partitionEntryRepository.GetPartitionEntries(addressId);
        }

        public PartitionEntry? GetPartitionEntry(int addressId, int partitionId)
        {
            return _partitionEntryRepository.GetPartitionEntry(addressId, partitionId);
        }

        public bool UpdatePartitionEntry(PartitionEntry entry)
        {
            return _partitionEntryRepository.UpdatePartitionEntry(entry);
        }
    }
}
