using Cerebro.Core.Abstractions.Background;
using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.Models.Dtos.Addresses;
using Microsoft.Extensions.Logging;

namespace Cerebro.Core.Services.Background
{
    public class AddressClusterSyncBackgroundService : BackgroundQueueServiceBase<AddressClusterScopeRequest>
    {
        private readonly ILogger<AddressClusterSyncBackgroundService> _logger;
        private readonly IClusterStateRepository _clusterStateRepository;

        public AddressClusterSyncBackgroundService(ILogger<AddressClusterSyncBackgroundService> logger, IClusterStateRepository clusterStateRepository)
        {
            _logger = logger;
            _clusterStateRepository = clusterStateRepository;
        }

        public override void Handle(AddressClusterScopeRequest request)
        {
            switch (request.AddressClusterScopeRequestState)
            {
                case AddressClusterScopeRequestState.AddressCreationRequested:
                    RequestAddressCreationInOtherNodes(request);
                    break;
                case AddressClusterScopeRequestState.AddressPartitionChangeRequested:
                    RequestAddressPartitionChangeInOtherNodes(request);
                    break;
                default:
                    break;
            }
        }

        private async void RequestAddressPartitionChangeInOtherNodes(AddressClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Partition change for [{request.AddressCreationRequest.Name}] to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    var success = await nodeClient.Value.RequestAddressPartitionChange(request.AddressCreationRequest.Alias, 
                        request.AddressCreationRequest.Settings.PartitionSettings.PartitionNumber, request.RequestedBy);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Address partition change failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestAddressCreationInOtherNodes(AddressClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Address creating for [{request.AddressCreationRequest.Name}] to neighbor nodes");

            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    var success = await nodeClient.Value.RequestAddressCreation(request);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Address creation failed at {nodeClient.Key}, request is saved temporary");
            }
        }
    }
}
