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
                case AddressClusterScopeRequestState.AddressDeletionRequested:
                    RequestAddressDeletionInOtherNodes(request);
                    break;
                case AddressClusterScopeRequestState.AddressPartitionChangeRequested:
                    RequestAddressPartitionChangeInOtherNodes(request);
                    break;
                case AddressClusterScopeRequestState.AddressReplicationSettingsChangeRequested:
                    RequestAddressReplicationSettingsChangeInOtherNodes(request);
                    break;
                case AddressClusterScopeRequestState.AddressRetentionSettingsChangeRequested:
                    RequestAddressRetentionSettingsChangeInOtherNodes(request);
                    break;
                case AddressClusterScopeRequestState.AddressSchemaSettingsChangeRequested:
                    RequestAddressSchemaSettingsChangeInOtherNodes(request);
                    break;
                case AddressClusterScopeRequestState.AddressStorageSettingsChangeRequested:
                    RequestAddressStorageSettingsChangeInOtherNodes(request);
                    break;
                default:
                    break;
            }

        }

        private async void RequestAddressDeletionInOtherNodes(AddressClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Address [{request.AddressCreationRequest.Name}] deletion to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    var success = await nodeClient.Value.RequestAddressDeletion(request.AddressCreationRequest.Alias);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Address [{request.AddressCreationRequest.Name}] deletion failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestAddressRetentionSettingsChangeInOtherNodes(AddressClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Retention settings change for [{request.AddressCreationRequest.Name}] to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    var success = await nodeClient.Value.RequestAddressRetentionSettingsChange(request.AddressCreationRequest.Alias,
                        request.AddressCreationRequest.Settings.RetentionSettings, request.RequestedBy);

                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Address [{request.AddressCreationRequest.Name}] retention settings change failed at {nodeClient.Key}, request is saved temporary");
            }

        }

        private async void RequestAddressSchemaSettingsChangeInOtherNodes(AddressClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Schema settings change for [{request.AddressCreationRequest.Name}] to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    var success = await nodeClient.Value.RequestAddressSchemaSettingsChange(request.AddressCreationRequest.Alias,
                        request.AddressCreationRequest.Settings.SchemaSettings, request.RequestedBy);

                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Address [{request.AddressCreationRequest.Name}] schema settings change failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestAddressStorageSettingsChangeInOtherNodes(AddressClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Storage settings change for [{request.AddressCreationRequest.Name}] to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    var success = await nodeClient.Value.RequestAddressStorageSettingsChange(request.AddressCreationRequest.Alias,
                        request.AddressCreationRequest.Settings.StorageSettings, request.RequestedBy);

                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Address [{request.AddressCreationRequest.Name}] storage settings change failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestAddressReplicationSettingsChangeInOtherNodes(AddressClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Replication settings change for [{request.AddressCreationRequest.Name}] to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    var success = await nodeClient.Value.RequestAddressReplicationSettingsChange(request.AddressCreationRequest.Alias,
                        request.AddressCreationRequest.Settings.ReplicationSettings, request.RequestedBy);

                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Address [{request.AddressCreationRequest.Name}] replication settings change failed at {nodeClient.Key}, request is saved temporary");
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

                _logger.LogWarning($"Address [{request.AddressCreationRequest.Name}] partition change failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestAddressCreationInOtherNodes(AddressClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Address creation for [{request.AddressCreationRequest.Name}] to neighbor nodes");
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

                _logger.LogWarning($"Address [{request.AddressCreationRequest.Name}] creation failed at {nodeClient.Key}, request is saved temporary");
            }
        }
    }
}
