using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.Models.Dtos.Clients;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Models.Entities.Clients.Applications;
using Cerebro.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Cerebro.Core.Services.ServerStates
{
    public class ClientConnectionService : IClientConnectionService
    {
        private readonly ILogger<ClientConnectionService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IApplicationRepository _applicationRepository;

        public ClientConnectionService(ILogger<ClientConnectionService> logger, IAddressRepository addressRepository, IApplicationRepository applicationRepository)
        {
            _logger = logger;
            _addressRepository = addressRepository;
            _applicationRepository = applicationRepository;
        }

        public (List<ClientConnectionDto>?, string message) GetClientConnectionsByAddressAlias(string addressAlias)
        {
            var address = _addressRepository.GetAddressByAlias(addressAlias);
            if (address == null)
                return (null, $"Address alias [{addressAlias}] doesnot exists");

            var clientConnections = _applicationRepository.GetClientConnectionsByAddress(address.Id)!;
            List<ClientConnectionDto> clientConenctionsDto = MapClientConnectionsByAddress(address, clientConnections);

            return (clientConenctionsDto, message: "Client connections returned");
        }


        public (List<ClientConnectionDto>?, string message) GetClientConnectionsByAddressName(string addressName)
        {
            var address = _addressRepository.GetAddressByName(addressName);
            if (address == null)
                return (null, $"Address [{addressName}] doesnot exists");

            var clientConnections = _applicationRepository.GetClientConnectionsByAddress(address.Id)!;
            List<ClientConnectionDto> clientConenctionsDto = MapClientConnectionsByAddress(address, clientConnections);

            return (clientConenctionsDto, message: "Client connections returned");
        }

        public (List<ClientConnectionDto>?, string message) GetClientConnectionsByApplicationName(string applicationName)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (null, $"Application [{applicationName}] doesnot exists");

            var clientConenctionsDto = _applicationRepository
                .GetClientConnectionsByApplication(application.Id)!
                .Select(a => new ClientConnectionDto()
                {
                    Id = a.Id,
                    Address = _addressRepository.GetAddressById(a.AddressId)!.Name,
                    ApplicationName = application.Name,
                    ApplicationConnectionType = a.ApplicationConnectionType,
                    ConnectedIPs = a.ConnectedIPs,
                    FirstConnectionDate = a.FirstConnectionDate,
                    IsConnected = a.IsConnected,
                    LastConnectionDate = a.LastConnectionDate,
                    ProductionInstanceType = a.ProductionInstanceType,
                    ReadInitialPosition = a.ReadInitialPosition,
                    SubscriptionMode = a.SubscriptionMode,
                    SubscriptionType = a.SubscriptionType
                }).ToList();


            return (clientConenctionsDto, message: "Client connections returned");
        }

        private List<ClientConnectionDto> MapClientConnectionsByAddress(Address? address, List<ClientConnection> clientConnections)
        {
            return clientConnections.Select(a => new ClientConnectionDto()
            {
                Id = a.Id,
                Address = address!.Name,
                ApplicationName = _applicationRepository.GetApplication(a.ApplicationId)!.Name,
                ApplicationConnectionType = a.ApplicationConnectionType,
                ConnectedIPs = a.ConnectedIPs,
                FirstConnectionDate = a.FirstConnectionDate,
                IsConnected = a.IsConnected,
                LastConnectionDate = a.LastConnectionDate,
                ProductionInstanceType = a.ProductionInstanceType,
                ReadInitialPosition = a.ReadInitialPosition,
                SubscriptionMode = a.SubscriptionMode,
                SubscriptionType = a.SubscriptionType
            }).ToList();
        }
    }
}
