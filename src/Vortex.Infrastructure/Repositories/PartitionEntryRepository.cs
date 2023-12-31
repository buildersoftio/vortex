﻿using Vortex.Core.Abstractions.Repositories;
using Vortex.Core.Models.Entities.Entries;
using Vortex.Infrastructure.DataAccess.IndexesState;
using Microsoft.Extensions.Logging;

namespace Vortex.Infrastructure.Repositories
{
    public class PartitionEntryRepository : IPartitionEntryRepository
    {
        private readonly ILogger<PartitionEntryRepository> _logger;
        private readonly IndexCatalogDbContext _indexCatalogDbContext;

        public PartitionEntryRepository(ILogger<PartitionEntryRepository> logger, IndexCatalogDbContext indexCatalogDbContext)
        {
            _logger = logger;
            _indexCatalogDbContext = indexCatalogDbContext;
        }

        public bool AddPartitionEntry(PartitionEntry partitionEntry)
        {
            var id = _indexCatalogDbContext.PartitionEntries!.Insert(partitionEntry);
            if (id != 0)
                return true;

            return false;
        }

        public bool UpdatePartitionEntry(PartitionEntry partitionEntry)
        {
            return _indexCatalogDbContext
                .PartitionEntries!
                .Update(partitionEntry);
        }

        public PartitionEntry? GetPartitionEntry(int addressId, int partitionId)
        {
            return _indexCatalogDbContext.PartitionEntries!.Query()
                .Where(x => x.AddressId == addressId && x.PartitionId == partitionId)
                .FirstOrDefault();
        }

        public List<PartitionEntry> GetPartitionEntries(int addressId)
        {
            return _indexCatalogDbContext.PartitionEntries!.Query()
                  .Where(x => x.AddressId == addressId)
                  .ToList();
        }

        public List<PartitionEntry> GetPartitionEntries(string addressAlias)
        {
            return _indexCatalogDbContext.PartitionEntries!.Query()
                  .Where(x => x.AddressAlias == addressAlias)
                  .ToList();
        }

        public bool DeletePartitionEntry(PartitionEntry partitionEntry)
        {
            return _indexCatalogDbContext
                .PartitionEntries!
                .Delete(partitionEntry.Id);
        }
    }
}
