using Vortex.Core.Abstractions.Factories;
using Vortex.Core.Abstractions.Repositories.Data;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Abstractions.Services.Data;
using Vortex.Core.Models.Data;
using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Models.Entities.Entries;

namespace Vortex.Core.Services.Data
{
    public class PartitionDataService : IPartitionDataService<Message>, IDisposable
    {
        private bool disposed = false;


        private readonly IPartitionEntryService _partitionEntryService;
        private readonly IPartitionDataFactory _partitionDataFactory;
        private readonly Address _address;
        private readonly PartitionEntry _partitionEntry;

        private readonly IPartitionDataRepository _partitionDataRepository;

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

            _partitionDataRepository = _partitionDataFactory
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _partitionDataRepository.CloseConnection();
                }
               
                disposed = true;
            }
        }
    }
}
