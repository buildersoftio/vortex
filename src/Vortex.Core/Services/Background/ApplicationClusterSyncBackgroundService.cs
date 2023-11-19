using Cerebro.Core.Abstractions.Background;
using Cerebro.Core.Abstractions.Clustering;
using Cerebro.Core.Abstractions.IO.Services;
using Cerebro.Core.Models.Dtos.Applications;
using Microsoft.Extensions.Logging;

namespace Cerebro.Core.Services.Background
{
    public class ApplicationClusterSyncBackgroundService : BackgroundQueueServiceBase<ApplicationClusterScopeRequest>
    {
        private readonly ILogger<ApplicationClusterSyncBackgroundService> _logger;
        private readonly IClusterStateRepository _clusterStateRepository;

        public ApplicationClusterSyncBackgroundService(ILogger<ApplicationClusterSyncBackgroundService> logger,
            IClusterStateRepository clusterStateRepository,
            ITemporaryIOService temporaryIOService) : base("applicationCluster_", temporaryIOService)
        {
            _logger = logger;
            _clusterStateRepository = clusterStateRepository;
        }

        public override void Handle(ApplicationClusterScopeRequest request)
        {

            switch (request.State)
            {
                case ApplicationClusterScopeRequestState.ApplicationCreationRequested:
                    RequestApplicationCreation(request);
                    break;
                case ApplicationClusterScopeRequestState.ApplicationSoftDeletionRequested:
                    RequestApplicationSoftDeletion(request);
                    break;
                case ApplicationClusterScopeRequestState.ApplicationHardDeletionRequested:
                    RequestApplicationHardDeletion(request);
                    break;
                case ApplicationClusterScopeRequestState.ApplicationDescriptionChangeRequest:
                    RequestApplicationDescriptionChange(request);
                    break;
                case ApplicationClusterScopeRequestState.ApplicationSettingsChangeRequest:
                    RequestApplicationSettingsChange(request);
                    break;
                case ApplicationClusterScopeRequestState.ApplicationActivationRequest:
                    RequestActivationRequest(request);
                    break;
                case ApplicationClusterScopeRequestState.ApplicationTokenCreationRequest:
                    RequestTokenCreation(request);
                    break;
                case ApplicationClusterScopeRequestState.ApplicationTokenRevocationRequest:
                    RequestTokenRevocation(request);
                    break;
                case ApplicationClusterScopeRequestState.ApplicationPermissionChangeRequest:
                    RequestPermissionChange(request);
                    break;
                default:
                    break;
            }

        }

        private async void RequestPermissionChange(ApplicationClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Application [{request.ApplicationDto.Name}] [{request.ApplicationPermissionKey}] permission change to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestApplicationPermissionChange(request.ApplicationDto.Name, request.ApplicationPermissionKey, request.ApplicationPermissionValue, request.RequestedBy);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                _logger.LogWarning($"Application [{request.ApplicationDto.Name}] permission change failed at {nodeClient.Key}, request is saved temporary");
                StoreFailedRequestInFile(request, nodeClient.Key);
            }
        }

        private async void RequestTokenRevocation(ApplicationClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Application [{request.ApplicationDto.Name}] token revocation [{request.ApplicationToken!.Id}] to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestApplicationTokenRevocation(request.ApplicationDto.Name, request.ApplicationToken!.Id, request.RequestedBy);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                StoreFailedRequestInFile(request, nodeClient.Key);
                _logger.LogWarning($"Application [{request.ApplicationDto.Name}] token revocation failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestTokenCreation(ApplicationClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Application [{request.ApplicationDto.Name}] token creation to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestApplicationTokenCreation(request.ApplicationDto.Name, request.ApplicationToken!, request.RequestedBy);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                StoreFailedRequestInFile(request, nodeClient.Key);
                _logger.LogWarning($"Application [{request.ApplicationDto.Name}] token creation failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestActivationRequest(ApplicationClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Application [{request.ApplicationDto.Name}] activation change to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestApplicationStatusChange(request.ApplicationDto.Name, request.ApplicationIsActive!.Value, request.RequestedBy);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                StoreFailedRequestInFile(request, nodeClient.Key);
                _logger.LogWarning($"Application [{request.ApplicationDto.Name}] activation change failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestApplicationSettingsChange(ApplicationClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Application [{request.ApplicationDto.Name}] settings change to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestApplicationSettingsChange(request.ApplicationDto.Name, request.ApplicationDto.Settings, request.RequestedBy);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                StoreFailedRequestInFile(request, nodeClient.Key);
                _logger.LogWarning($"Application [{request.ApplicationDto.Name}] settings change failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestApplicationDescriptionChange(ApplicationClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Application [{request.ApplicationDto.Name}] description change to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestApplicationDescriptionChange(request.ApplicationDto.Name, request.ApplicationDto.Description, request.RequestedBy);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                StoreFailedRequestInFile(request, nodeClient.Key);
                _logger.LogWarning($"Application [{request.ApplicationDto.Name}] description change failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestApplicationHardDeletion(ApplicationClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Application [{request.ApplicationDto.Name}] hard deletion to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestApplicationHardDeletion(request.ApplicationDto.Name);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                StoreFailedRequestInFile(request, nodeClient.Key);
                _logger.LogWarning($"Application [{request.ApplicationDto.Name}] hard deletion failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestApplicationSoftDeletion(ApplicationClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Application [{request.ApplicationDto.Name}] soft deletion to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestApplicationSoftDeletion(request.ApplicationDto.Name, request.RequestedBy);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                StoreFailedRequestInFile(request, nodeClient.Key);
                _logger.LogWarning($"Application [{request.ApplicationDto.Name}] soft deletion failed at {nodeClient.Key}, request is saved temporary");
            }
        }

        private async void RequestApplicationCreation(ApplicationClusterScopeRequest request)
        {
            _logger.LogInformation($"Requesting Application [{request.ApplicationDto.Name}] creation to neighbor nodes");
            foreach (var nodeClient in _clusterStateRepository.GetNodeClients())
            {
                try
                {
                    // if this request is stored temporary
                    if (request.NodeId != null && request.NodeId != nodeClient.Key)
                        continue;

                    var success = await nodeClient.Value.RequestApplicationCreation(request.ApplicationDto, request.RequestedBy);
                    if (success == true)
                        continue;
                }
                catch (Exception)
                {

                }

                StoreFailedRequestInFile(request, nodeClient.Key);
                _logger.LogWarning($"Application [{request.ApplicationDto.Name}] creation failed at {nodeClient.Key}, request is saved temporary");
            }
        }


        private void StoreFailedRequestInFile(ApplicationClusterScopeRequest request, string nodeId)
        {
            StoreFailedRequest(new ApplicationClusterScopeRequest()
            {
                NodeId = nodeId,
                RequestedBy = request.RequestedBy,
                ApplicationDto = request.ApplicationDto,
                ApplicationIsActive = request.ApplicationIsActive,
                ApplicationPermissionKey = request.ApplicationPermissionKey,
                ApplicationPermissionValue = request.ApplicationPermissionValue,
                ApplicationToken = request.ApplicationToken,
                State = request.State
            });
        }
    }
}
