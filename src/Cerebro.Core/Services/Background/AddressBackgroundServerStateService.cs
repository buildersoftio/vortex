using Cerebro.Core.Abstractions.Background;
using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.Abstractions.Factories;
using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.IO;
using Cerebro.Core.IO.Services;
using Cerebro.Core.Models.BackgroundRequests;
using Cerebro.Core.Models.Common.Addresses;
using Cerebro.Core.Models.Common.Clusters;
using Cerebro.Core.Models.Configurations;
using Cerebro.Core.Models.Dtos.Addresses;
using Cerebro.Core.Repositories;
using Cerebro.Core.Services.Data;
using Cerebro.Core.Utilities.Extensions;
using Microsoft.Extensions.Logging;

namespace Cerebro.Core.Services.Background
{
    public class AddressBackgroundServerStateService : SimpleBackgroundQueueServiceBase<AddressBackgroundRequest>
    {
        private readonly ILogger<AddressBackgroundServerStateService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IPartitionEntryService _partitionEntryService;
        private readonly IDataIOService _dataIOService;
        private readonly IPartitionDataFactory _partitionDataFactory;
        private readonly IClusterStateRepository _clusterStateRepository;
        private readonly NodeConfiguration _nodeConfiguration;

        private readonly IBackgroundQueueService<AddressClusterScopeRequest> _backgroundAddressClusterService;

        public AddressBackgroundServerStateService(ILogger<AddressBackgroundServerStateService> logger,
            IAddressRepository addressRepository,
            IPartitionEntryService partitionEntryService,
            IDataIOService dataIOService,
            IPartitionDataFactory partitionDataFactory,
            IClusterStateRepository clusterStateRepository,
            NodeConfiguration nodeConfiguration,
            IBackgroundQueueService<AddressClusterScopeRequest> backgroundAddressClusterService)
        {
            _logger = logger;
            _addressRepository = addressRepository;
            _partitionEntryService = partitionEntryService;
            _dataIOService = dataIOService;
            _partitionDataFactory = partitionDataFactory;

            // resources needed for clustering and node owners
            _clusterStateRepository = clusterStateRepository;
            _nodeConfiguration = nodeConfiguration;

            _backgroundAddressClusterService = backgroundAddressClusterService;
        }

        public override void Handle(AddressBackgroundRequest request)
        {
            // use base.EnqueueRequest for each change in status, in this form we can control the narrative of creating the address.
            if (request.Address.Status == AddressStatuses.CreateAddressDirectory)
            {
                // Create the directory and RocksDb Databases for all partitions
                if (_dataIOService.IsAddressDirCreated(request.Address.Id) != true)
                {
                    if (_dataIOService.CreateAddressDir(request.Address.Id))
                    {
                        request.Address.Status = AddressStatuses.CreatePartitionDirectories;
                        _addressRepository.UpdateAddress(request.Address);

                        _logger.LogInformation($"Address [{request.Address.Name}] initializing partitions");
                    }

                    base.EnqueueRequest(request);
                    return;
                }
            }

            if (request.Address.Status == AddressStatuses.CreatePartitionDirectories || request.Address.Status == AddressStatuses.ChangePartitions)
            {
                var nodesInCluster = _clusterStateRepository.GetNodes().Values.ToList();

                // Inserting this node, for easy implementation of nodeOwnership for Partition Creation
                nodesInCluster.Insert(0, new Node() { Id = _nodeConfiguration.NodeId });


                int partitionCount = request.Address.Settings.PartitionSettings.PartitionNumber;

                for (int i = 0; i < partitionCount; i++)
                {
                    if (_dataIOService.IsAddressPartitionDirCreated(request.Address.Id, i) != true)
                    {
                        // we are creating the index and state store for the partitions
                        var partitionCreated = _partitionEntryService.CreatePartitionEntry(request.Address.Id, request.Address.Alias, i, request.Address.Settings.MessageIndexType, request.Address.CreatedBy!);

                        if (partitionCreated)
                        {
                            _dataIOService.CreateAddressPartitionDir(request.Address.Id, i);
                            // After creating the directories, we have to create the partition storage files.
                            var partitionEntry = _partitionEntryService.GetPartitionEntry(request.Address.Id, i);
                            if (partitionEntry != null)
                            {
                                _logger.LogInformation($"Address [{request.Address.Name}] partition [{i}] creating storage files");
                                using var partitionDbData = new PartitionDataService(_partitionEntryService, _partitionDataFactory, request.Address, partitionEntry!);
                                _logger.LogInformation($"Address [{request.Address.Name}] partition [{i}] storage files created");
                            }
                        }
                        else
                        {
                            // here we have a bug, in case the creation fails, we should retry the the re-creation by trying the same request again.

                            // in case of failing, we are retying the same request again.
                            request.Address.Status = AddressStatuses.Failed;
                            _addressRepository.UpdateAddress(request.Address);
                            return;
                        }
                    }

                    if (request.Address.Settings.PartitionSettings.Partitions.ContainsKey(i) != true)
                    {
                        string nodeOwner = _nodeConfiguration.NodeId;
                        if (request.Address.Settings.Scope == AddressScope.ClusterScope)
                        {
                            nodeOwner = FindNodeOwner(request.Address.Settings.PartitionSettings.Partitions, i, nodesInCluster);
                        }

                        request.Address.Settings.PartitionSettings.Partitions.Add(i, new Partition()
                        {
                            IsActive = true,
                            NodeOwner = nodeOwner,
                            DataLocation = DataLocations.GetAddressPartitionDirectory(request.Address.Id, i).GetLastThreeLevels()
                        });
                    }

                    // updating entry position
                    var entryPosition = _partitionEntryService.GetPartitionEntry(request.Address.Id, i);
                    entryPosition!.NodeOwner = request.Address.Settings.PartitionSettings.Partitions[i].NodeOwner;
                    _partitionEntryService.UpdatePartitionEntry(entryPosition);
                }

                _logger.LogInformation($"Address [{request.Address.Name}] partitions intialized successfully");


                // inform other nodes to create the new address in case address is new..
                if (request.Address.Status == AddressStatuses.CreatePartitionDirectories)
                {
                    // in case of clusterScope inform other nodes to create the address with these settings....
                    if (request.Address.Settings.Scope == AddressScope.ClusterScope && request.IsRequestedFromOtherNode != true)
                    {
                        _backgroundAddressClusterService.EnqueueRequest(new AddressClusterScopeRequest()
                        {
                            AddressCreationRequest = new AddressCreationRequest() { Alias = request.Address.Alias, Name = request.Address.Name, Settings = request.Address.Settings },
                            AddressClusterScopeRequestState = AddressClusterScopeRequestState.AddressCreationRequested,
                            RequestedBy = request.Address.CreatedBy!
                        });
                    }
                }

                if (request.Address.Status == AddressStatuses.ChangePartitions)
                {
                    // in case of cluster scope, inform other nodes
                    if (request.Address.Settings.Scope == AddressScope.ClusterScope && request.IsRequestedFromOtherNode != true)
                    {
                        _backgroundAddressClusterService.EnqueueRequest(new AddressClusterScopeRequest()
                        {
                            AddressCreationRequest = new AddressCreationRequest() { Alias = request.Address.Alias, Name = request.Address.Name, Settings = request.Address.Settings },
                            AddressClusterScopeRequestState = AddressClusterScopeRequestState.AddressPartitionChangeRequested,
                            RequestedBy = request.Address.UpdatedBy!
                        });
                    }
                }


                request.Address.Status = AddressStatuses.Ready;
                _addressRepository.UpdateAddress(request.Address);
            }

            if (request.Address.Status == AddressStatuses.DeletePartitions)
            {
                _logger.LogInformation($"Address [{request.Address.Name}] partition deletion requested");

                if (_partitionEntryService.DeletePartitionEntries(request.Address.Id))
                {
                    _logger.LogInformation($"Address [{request.Address.Name}] partition deletion completed");
                    return;
                }

                base.EnqueueRequest(request);
                return;
            }
        }

        private string FindNodeOwner(Dictionary<int, Partition> partitions, int i, List<Node> nodesInCluster)
        {
            if (i == 0)
                return _nodeConfiguration.NodeId;

            string lastNodeOwner = partitions[i - 1].NodeOwner;
            int nodeIndex = nodesInCluster.FindIndex(x => x.Id == lastNodeOwner);

            if (nodeIndex >= nodesInCluster.Count - 1)
            {
                nodeIndex = -1; // Restart from the beginning of the node list if we run out of nodes
            }

            nodeIndex++;

            return nodesInCluster[nodeIndex].Id;
        }
    }
}
