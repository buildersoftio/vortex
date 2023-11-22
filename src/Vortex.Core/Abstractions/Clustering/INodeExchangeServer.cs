namespace Vortex.Core.Abstractions.Clustering
{
    public interface INodeExchangeServer
    {
        void Start();
        Task ShutdownAsync();

    }
}
