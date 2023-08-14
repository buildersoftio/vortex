namespace Cerebro.Core.Abstractions.Clustering
{
    public interface INodeExchangeClient
    {
        Task<bool> ConnectAsync();
        Task DisconnectAsync();

        Task<bool> RequestHeartBeatAsync();
    }
}