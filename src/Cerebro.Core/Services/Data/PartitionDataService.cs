using Cerebro.Core.Abstractions.Factories;
using Cerebro.Core.Abstractions.Repositories;
using Cerebro.Core.Abstractions.Repositories.Data;
using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.Abstractions.Services.Data;
using Cerebro.Core.Models.Configurations;
using Cerebro.Core.Models.Data;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Models.Entities.Entries;

namespace Cerebro.Core.Services.Data
{
    public class PartitionDataService : IPartitionDataService<Message>
    {
        private readonly IPartitionEntryService _partitionEntryService;
        private readonly IPartitionDataFactory _partitionDataFactory;
        private readonly Address _address;
        private readonly PartitionEntry _partitionEntry;

        private readonly IPartitionDataRepository partitionDataRepository;

        public PartitionDataService(
            IPartitionEntryService partitionEntryService,
            IPartitionDataFactory partitionDataFactory,
            Address address,
            PartitionEntry partitionEntry)
        {
            _partitionEntryService = partitionEntryService;
            _partitionDataFactory = partitionDataFactory;
            _address = address;
            _partitionEntry = partitionEntry;

            partitionDataRepository = _partitionDataFactory
                .CreatePartitionDataRepository(address, partitionEntry);
        }

        public void Delete(long entryId)
        {
            throw new NotImplementedException();
        }

        public Message Get(long entryId)
        {
            throw new NotImplementedException();
        }

        public Message GetNext(long entryId)
        {
            throw new NotImplementedException();
        }

        public void Put(long entryId, Message entity)
        {
            throw new NotImplementedException();
        }

        public bool TryGet(long entryId, out Message entity)
        {
            throw new NotImplementedException();
        }

        public bool TryGetNext(long entryId, out Message entity)
        {
            throw new NotImplementedException();
        }
    }
}
