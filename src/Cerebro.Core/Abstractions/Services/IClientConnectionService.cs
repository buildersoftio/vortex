using Cerebro.Core.Models.Dtos.Clients;

namespace Cerebro.Core.Abstractions.Services
{
    public interface IClientConnectionService
    {
        (List<ClientConnectionDto>?, string message) GetClientConnectionsByApplicationName(string applicationName);
        (List<ClientConnectionDto>?, string message) GetClientConnectionsByAddressName(string addressName);
        (List<ClientConnectionDto>?, string message) GetClientConnectionsByAddressAlias(string addressAlias);
    }
}
