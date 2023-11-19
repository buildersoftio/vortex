using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Models.Common.Addresses;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Common.Clusters;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Dtos.Addresses;
using Vortex.Core.Models.Dtos.Applications;
using Vortex.Core.Models.Entities.Clients.Applications;
using Vortex.Core.Utilities.Json;
using Grpc.Core;

namespace Vortex.Cluster.Infrastructure.Clients
{
    public class gRPCNodeExchangeClient : INodeExchangeClient
    {
        private readonly Node _node;
        private readonly NodeConfiguration _serverNodeConfiguration;
        private readonly Channel _channel;
        private readonly NodeExchange.NodeExchangeService.NodeExchangeServiceClient _client;

        public gRPCNodeExchangeClient(Node node, NodeConfiguration nodeConfiguration)
        {
            _node = node;
            _serverNodeConfiguration = nodeConfiguration;

            _channel = new Channel($"{node.Address}:{node.Port}", ChannelCredentials.Insecure);
            _client = new NodeExchange.NodeExchangeService.NodeExchangeServiceClient(_channel);
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                var response = await _client.RegisterNodeAsync(new NodeExchange.NodeInfo() { NodeId = _serverNodeConfiguration.NodeId });
                if (response.Success != true)
                    return false;

                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }

        public Task DisconnectAsync()
        {
            //TODO: figure it out how to disconnect the client
            return Task.CompletedTask;
        }

        #region Address Sync Region

        public async Task<bool> RequestAddressCreation(AddressClusterScopeRequest request)
        {
            try
            {
                var response = await _client.RequestAddressCreationAsync(new NodeExchange.AddressCreationRequest()
                {
                    Address = request.AddressCreationRequest.ToJson(),
                    AddressClusterScopeRequestState = (int)request.AddressClusterScopeRequestState,
                    CreatedBy = request.RequestedBy
                });
                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestAddressDeletion(string alias)
        {
            try
            {
                var response = await _client.RequestAddressDeletionAsync(new NodeExchange.AddressDeletionRequest() { Alias = alias });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestAddressPartitionChange(string alias, int partitionNumber, string updatedBy)
        {
            try
            {
                var response = await _client.RequestAddressPartitionChangeAsync(new NodeExchange.AddressPartitionChangeRequest()
                {
                    Alias = alias,
                    PartitionNumber = partitionNumber,
                    UpdatedBy = updatedBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestAddressReplicationSettingsChange(string alias, AddressReplicationSettings addressReplicationSettings, string updatedBy)
        {
            try
            {
                var response = await _client.RequestAddressReplicationSettingsChangeAsync(new NodeExchange.AddressReplicationSettingsChangeRequest()
                {
                    Alias = alias,
                    ReplicationSettingsJson = addressReplicationSettings.ToJson(),
                    UpdatedBy = updatedBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestAddressRetentionSettingsChange(string alias, AddressRetentionSettings addressRetentionSettings, string updatedBy)
        {
            try
            {
                var response = await _client.RequestAddressRetentionSettingsChangeAsync(new NodeExchange.AddressRetentionSettingsChangeRequest()
                {
                    Alias = alias,
                    RetentionSettingsJson = addressRetentionSettings.ToJson(),
                    UpdatedBy = updatedBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestAddressSchemaSettingsChange(string alias, AddressSchemaSettings addressSchemaSettings, string updatedBy)
        {
            try
            {
                var response = await _client.RequestAddressSchemaSettingsChangeAsync(new NodeExchange.AddressSchemaSettingsChangeRequest()
                {
                    Alias = alias,
                    SchemaSettingsJson = addressSchemaSettings.ToJson(),
                    UpdatedBy = updatedBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestAddressStorageSettingsChange(string alias, AddressStorageSettings addressStorageSettings, string updatedBy)
        {
            try
            {
                var response = await _client.RequestAddressStorageSettingsChangeAsync(new NodeExchange.AddressStorageSettingsChangeRequest()
                {
                    Alias = alias,
                    StorageSettingsJson = addressStorageSettings.ToJson(),
                    UpdatedBy = updatedBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion


        #region Application Sync Region
        public async Task<bool> RequestApplicationCreation(ApplicationDto applicationDto, string createdBy)
        {
            try
            {
                var response = await _client.RequestApplicationCreationAsync(new NodeExchange.ApplicationCreationRequest()
                {
                    Name = applicationDto.Name,
                    Description = applicationDto.Description,
                    SettingsJson = applicationDto.Settings.ToJson(),
                    CreatedBy = createdBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestApplicationDescriptionChange(string applicationName, string description, string updatedBy)
        {
            try
            {
                var response = await _client.RequestApplicationUpdateAsync(new NodeExchange.ApplicationUpdateRequest()
                {
                    Name = applicationName,
                    Description = description,
                    SettingsJson = "DESCRIPTION_ONLY",
                    UpdatedBy = updatedBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestApplicationSettingsChange(string applicationName, ApplicationSettings applicationSettings, string updatedBy)
        {
            try
            {
                var response = await _client.RequestApplicationUpdateAsync(new NodeExchange.ApplicationUpdateRequest()
                {
                    Name = applicationName,
                    Description = "SETTINGS_ONLY",
                    SettingsJson = applicationSettings.ToJson(),
                    UpdatedBy = updatedBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestApplicationHardDeletion(string applicationName)
        {
            try
            {
                var response = await _client.RequestApplicationDeletionAsync(new NodeExchange.ApplicationDeletionRequest()
                {
                    ApplicationName = applicationName,
                    IsHardDelete = true,
                    UpdatedBy = "na"
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestApplicationSoftDeletion(string applicationName, string updatedBy)
        {
            try
            {
                var response = await _client.RequestApplicationDeletionAsync(new NodeExchange.ApplicationDeletionRequest()
                {
                    ApplicationName = applicationName,
                    IsHardDelete = false,
                    UpdatedBy = updatedBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestApplicationStatusChange(string applicationName, bool status, string updatedBy)
        {
            try
            {
                var response = await _client.RequestApplicationStatusChangeAsync(new NodeExchange.ApplicationActivationRequest()
                {
                    Name = applicationName,
                    IsActive = status,
                    UpdatedBy = updatedBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestApplicationTokenCreation(string applicationName, ApplicationToken applicationToken, string createdBy)
        {
            try
            {
                var response = await _client.RequestApplicationTokenCreationAsync(new NodeExchange.AddApplicationTokenRequest()
                {
                    ApplicationName = applicationName,
                    ApplicationTokenEntityJson = applicationToken.ToJson(),
                    CreatedBy = createdBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestApplicationTokenRevocation(string applicationName, Guid apiKey, string updatedBy)
        {
            try
            {
                var response = await _client.RequestApplicationTokenRevocationAsync(new NodeExchange.RevokeApplicationTokenRequest()
                {
                    ApplicationName = applicationName,
                    ApiKey = apiKey.ToString(),
                    UpdatedBy = updatedBy
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RequestApplicationPermissionChange(string applicationName, string permissionType, string value, string updatedBy)
        {
            try
            {
                var response = await _client.RequestApplicationPermissionChangeAsync(new NodeExchange.ChangeApplicationPermissionRequest()
                {
                    ApplicationName = applicationName,
                    PermissionType = permissionType,
                    UpdatedBy = updatedBy,
                    Value = value
                });

                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        public async Task<bool> RequestHeartBeatAsync()
        {
            var result = await _client.SendHeartbeatAsync(new NodeExchange.Heartbeat() { NodeId = _serverNodeConfiguration.NodeId });
            return true;
        }

    }
}
