using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.Factories;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.IO;
using Vortex.Core.IO.Services;
using Vortex.Core.Models.BackgroundRequests;
using Vortex.Core.Models.Common.Addresses;
using Vortex.Core.Models.Common.Clusters;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Dtos.Addresses;
using Vortex.Core.Repositories;
using Vortex.Core.Services.Data;
using Vortex.Core.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Contracts;

namespace Vortex.Core.Services.Background
{
    public class AddressBackgroundServerStateService : SimpleBackgroundQueueServiceBase<AddressBackgroundRequest>
    {
        private readonly ILogger<AddressBackgroundServerStateService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IPartitionEntryService _partitionEntryService;
        private readonly IDataIOService _dataIOService;
        private readonly IPartitionDataFactory _partitionDataFactory;
        private readonly IClusterStateRepository _clusterStateRepository;
        private readonly ISubscriptionEntryService _subscriptionEntryService;
        private readonly IApplicationRepository _applicationRepository;
        private readonly NodeConfiguration _nodeConfiguration;

        private readonly IBackgroundQueueService<AddressClusterScopeRequest> _backgroundAddressClusterService;

        public AddressBackgroundServerStateService(ILogger<AddressBackgroundServerStateService> logger,
            IAddressRepository addressRepository,
            IPartitionEntryService partitionEntryService,
            IDataIOService dataIOService,
            IPartitionDataFactory partitionDataFactory,
            IClusterStateRepository clusterStateRepository,
            ISubscriptionEntryService subscriptionEntryService,
            IApplicationRepository applicationRepository,
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
            _subscriptionEntryService = subscriptionEntryService;
            _applicationRepository = applicationRepository;
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
                List<int> applicationsConnected = _applicationRepository.GetClientConnectionsByAddress(request.Address.Id)!
                    .Where(x => x.ApplicationConnectionType == Models.Common.Clients.Applications.ApplicationConnectionTypes.Consumption)
                    .Select(x => x.ApplicationId).ToList();

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

                            CreateSubscriptionEntryForPartition(request, applicationsConnected, i);
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

                _logger.LogInformation($"Address [{request.Address.Name}] partitions initialized successfully");


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

        private void CreateSubscriptionEntryForPartition(AddressBackgroundRequest request, List<int> applicationsConnected, int partitionId)
        {
            // update subscription states for new partition added to the address.
            foreach (var applicationId in applicationsConnected)
            {
                var subscriptions = _subscriptionEntryService.GetSubscriptions(applicationId, request.Address.Id);
                foreach (var subscription in subscriptions)
                {
                    var previousSubscriptionEntry = _subscriptionEntryService.GetSubscriptionEntries(subscription, applicationId, request.Address.Id).FirstOrDefault();

                    _subscriptionEntryService
                        .CreateSubscriptionEntry(subscription, applicationId, request.Address.Id, partitionId, request.Address.Alias, previousSubscriptionEntry!.ConsumptionSettings, createdBy: "bg_PartitionCreationService");
                }
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
