using Cerebro.Core.Abstractions.Background;
using Cerebro.Core.Abstractions.Factories;
using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.IO;
using Cerebro.Core.IO.Services;
using Cerebro.Core.Models.Common.Addresses;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Repositories;
using Cerebro.Core.Services.Data;
using Cerebro.Core.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Cerebro.Core.Services.Background
{
    public class AddressBackgroundServerStateService : BackgroundQueueServiceBase<Address>
    {
        private readonly ILogger<AddressBackgroundServerStateService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IPartitionEntryService _partitionEntryService;
        private readonly IDataIOService _dataIOService;
        private readonly IPartitionDataFactory _partitionDataFactory;

        public AddressBackgroundServerStateService(ILogger<AddressBackgroundServerStateService> logger,
            IAddressRepository addressRepository,
            IPartitionEntryService partitionEntryService,
            IDataIOService dataIOService,
            IPartitionDataFactory partitionDataFactory)
        {
            _logger = logger;
            _addressRepository = addressRepository;
            _partitionEntryService = partitionEntryService;
            _dataIOService = dataIOService;
            _partitionDataFactory = partitionDataFactory;
        }

        public override void Handle(Address request)
        {
            // ADD states for each step, create_directories, create_rocks_db_for_partitions, create_replication_state_storage, create_indexes_state;
            // use base.EnqueueRequest for each change in status, in this form we can control the narrative of creating the address.
            if (request.Status == AddressStatuses.CreateAddressDirectory)
            {
                // Create the directory and RocksDb Databases for all partitions
                if (_dataIOService.IsAddressDirCreated(request.Id) != true)
                {
                    if (_dataIOService.CreateAddressDir(request.Id))
                    {
                        request.Status = AddressStatuses.CreatePartitionDirectories;
                        _addressRepository.UpdateAddress(request);

                        _logger.LogInformation($"Address [{request.Name}] initializing partitions");
                    }

                    base.EnqueueRequest(request);
                    return;
                }
            }

            if (request.Status == AddressStatuses.CreatePartitionDirectories || request.Status == AddressStatuses.ChangePartitions)
            {
                int partitionCount = request.Settings.PartitionSettings.PartitionNumber;

                for (int i = 0; i < partitionCount; i++)
                {
                    if (_dataIOService.IsAddressPartitionDirCreated(request.Id, i) != true)
                    {
                        // we are creating the index and state store for the partitions
                        var partitionCreated = _partitionEntryService.CreatePartitionEntry(request.Id, request.Alias, i, request.Settings.MessageIndexType, request.CreatedBy!);

                        if (partitionCreated)
                        {
                            _dataIOService.CreateAddressPartitionDir(request.Id, i);
                            // After creating the directories, we have to create the partition storage files.
                            var partitionEntry = _partitionEntryService.GetPartitionEntry(request.Id, i);
                            if (partitionEntry != null)
                            {
                                _logger.LogInformation($"Address [{request.Name}] partition [{i}] creating storage files");
                                var partitionDbData = new PartitionDataService(_partitionEntryService, _partitionDataFactory, request, partitionEntry!);
                                _logger.LogInformation($"Address [{request.Name}] partition [{i}] storage files created");
                            }
                        }
                        else
                        {
                            // In case of failing, we are retying the same request again.
                            request.Status = AddressStatuses.Ready;
                            _addressRepository.UpdateAddress(request);
                            return;
                        }
                    }

                    if (request.Settings.PartitionSettings.Partitions.ContainsKey(i) != true)
                    {
                        request.Settings.PartitionSettings.Partitions.Add(i, new Partition()
                        {
                            IsActive = true,
                            DataLocation = DataLocations.GetAddressPartitionDirectory(request.Id, i).GetLastThreeLevels()
                        });
                    }
                }

                _logger.LogInformation($"Address [{request.Name}] partitions intialized successfully");

                request.Status = AddressStatuses.Ready;
                _addressRepository.UpdateAddress(request);
            }
        }
    }
}
