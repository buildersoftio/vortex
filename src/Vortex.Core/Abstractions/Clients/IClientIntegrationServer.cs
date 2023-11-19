namespace Vortex.Core.Abstractions.Clients
{
    public interface IClientIntegrationServer
    {
        void Start();
        Task ShutdownAsync();
    }
}
