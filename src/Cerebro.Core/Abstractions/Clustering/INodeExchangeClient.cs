using Cerebro.Core.Models.Dtos.Addresses;

namespace Cerebro.Core.Abstractions.Clustering
{
    public interface INodeExchangeClient
    {
        Task<bool> ConnectAsync();
        Task DisconnectAsync();

        Task<bool> RequestHeartBeatAsync();

        Task<bool> RequestAddressCreation(AddressClusterScopeRequest request);
        Task<bool> RequestAddressPartitionChange(string alias, int partitionNumner, string updatedBy);
    }
}