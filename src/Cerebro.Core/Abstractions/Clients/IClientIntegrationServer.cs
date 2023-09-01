namespace Cerebro.Core.Abstractions.Clients
{
    public interface IClientIntegrationServer
    {
        void Start();
        Task ShutdownAsync();
    }
}
