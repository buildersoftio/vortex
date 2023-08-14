namespace Cerebro.Core.Abstractions.Clustering
{
    public interface INodeExchangeServer
    {
        void Start();
        Task ShutdownAsync();

    }
}
