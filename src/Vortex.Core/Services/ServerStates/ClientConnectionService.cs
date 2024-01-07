using Vortex.Core.Abstractions.Services;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Dtos.Clients;
using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Models.Entities.Clients.Applications;
using Vortex.Core.Repositories;
using Vortex.Core.Utilities.Consts;
using Microsoft.Extensions.Logging;

namespace Vortex.Core.Services.ServerStates
{
    public class ClientConnectionService : IClientConnectionService
    {
        private readonly ILogger<ClientConnectionService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IAddressService _addressService;

        public ClientConnectionService(ILogger<ClientConnectionService> logger, IAddressRepository addressRepository, IApplicationRepository applicationRepository, IAddressService addressService)
        {
            _logger = logger;
            _addressRepository = addressRepository;
            _applicationRepository = applicationRepository;

            _addressService = addressService;
        }

        public (List<ClientConnectionDto>? clientConnections, string message) GetClientConnectionsByAddressAlias(string addressAlias)
        {
            var address = _addressRepository.GetAddressByAlias(addressAlias);
            if (address == null)
                return (null, $"Address alias [{addressAlias}] doesnot exists");

            var clientConnections = _applicationRepository.GetClientConnectionsByAddress(address.Id)!;
            List<ClientConnectionDto> clientConenctionsDto = MapClientConnectionsByAddress(address, clientConnections);

            return (clientConenctionsDto, message: "Client connections returned");
        }

        public (List<ClientConnectionDto>? clientConnections, string message) GetClientConnectionsByAddressName(string addressName)
        {
            var address = _addressRepository.GetAddressByName(addressName);
            if (address == null)
                return (null, $"Address [{addressName}] doesnot exists");

            var clientConnections = _applicationRepository.GetClientConnectionsByAddress(address.Id)!;
            List<ClientConnectionDto> clientConnectionsDto = MapClientConnectionsByAddress(address, clientConnections);

            return (clientConnectionsDto, message: "Client connections returned");
        }

        public (List<ClientConnectionDto>? clientConnections, string message) GetClientConnectionsByApplicationName(string applicationName)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (null, $"Application [{applicationName}] doesnot exists");

            var clientConnectionsDto = _applicationRepository
                .GetClientConnectionsByApplication(application.Id)!
                .Select(a => new ClientConnectionDto()
                {
                    Id = a.Id,
                    Address = _addressRepository.GetAddressById(a.AddressId)!.Name,
                    ApplicationName = application.Name,
                    ApplicationConnectionType = a.ApplicationConnectionType,
                    ConnectedHosts = a.ConnectedHosts,
                    HostsHistory = a.HostsHistory,
                    FirstConnectionDate = a.FirstConnectionDate,
                    IsConnected = a.IsConnected,
                    LastConnectionDate = a.LastConnectionDate,
                    ProductionInstanceType = a.ProductionInstanceType,
                    ConsumptionSettings = a.ConsumptionSettings
                }).ToList();


            return (clientConnectionsDto, message: "Client connections returned");
        }


        public (bool status, string message) RegisterClientConnection(ClientConnectionRequest clientConnectionRequest, string createdBy)
        {
            var application = _applicationRepository.GetApplication(clientConnectionRequest.ApplicationName);
            if (application == null)
                return (status: false, message: $"Application [{clientConnectionRequest.ApplicationName}] doesnot exists");

            var applicationPermission = _applicationRepository.GetApplicationPermission(application.Id);

            var address = _addressRepository.GetAddressByName(clientConnectionRequest.Address);
            if (address == null)
            {
                if (applicationPermission.Permissions![DefaultApplicationPermissions.CREATE_ADDRESS_PERMISSION_KEY] != "True")
                    return (status: false, message: $"Address [{clientConnectionRequest.Address}] doesnot exists");

                string addressAlias = clientConnectionRequest.Address.Replace(" ", "").Replace("/", "");
                (var status, string msg) = _addressService.CreateDefaultAddress(new Models.Dtos.Addresses.AddressDefaultCreationRequest()
                {
                    Alias = addressAlias,
                    Name = clientConnectionRequest.Address,
                    PartitionNumber = 1
                }, createdBy);

                if (status == false)
                    return (status: false, message: msg);

                address = _addressRepository.GetAddressByName(clientConnectionRequest.Address);
            }


            var clientConnection = _applicationRepository.GetClientConnection(application.Id, address!.Id, clientConnectionRequest.ApplicationConnectionType);
            if (clientConnection != null)
                return (status: true, message: $"Application [{clientConnectionRequest.ApplicationName}] is already integrated with address [{clientConnectionRequest.Address}]");

            clientConnection = new ClientConnection()
            {
                Id = Guid.NewGuid(),
                AddressId = address!.Id,
                ApplicationId = application.Id,
                ApplicationConnectionType = clientConnectionRequest.ApplicationConnectionType,
                IsConnected = false,
                ConnectedHosts = new List<string>(),
                HostsHistory = new Dictionary<string, ApplicationHost>(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            if (clientConnectionRequest.ApplicationConnectionType == ApplicationConnectionTypes.Production)
            {
                if (IsReadOrWritePermission(applicationPermission.Permissions![DefaultApplicationPermissions.WRITE_ADDRESS_PERMISSION_KEY], clientConnectionRequest.Address) != true)
                    return (status: false, message: $"Application [{clientConnectionRequest.ApplicationName}] cannot write to address [{clientConnectionRequest.Address}], connection cannot be established");

                if (clientConnectionRequest.ProductionInstanceType == null)
                    return (status: false, message: $"ProductionInstanceType can not be null");

                clientConnection.ProductionInstanceType = clientConnectionRequest.ProductionInstanceType!.Value;
            }

            else
            {
                if (IsReadOrWritePermission(applicationPermission.Permissions![DefaultApplicationPermissions.READ_ADDRESS_PERMISSION_KEY], clientConnectionRequest.Address) != true)
                    return (status: false, message: $"Application [{clientConnectionRequest.ApplicationName}] cannot read from address [{clientConnectionRequest.Address}], connection cannot be established");


                if (clientConnectionRequest.ConsumptionSettings == null)
                    return (status: false, message: $"ConsumptionSettings can not be null");
                clientConnection.ConsumptionSettings = clientConnectionRequest.ConsumptionSettings!;
            }

            if (_applicationRepository.AddApplicationAddressConnection(clientConnection))
                return (status: true, message: $"Application [{clientConnectionRequest.ApplicationName}] is integrated with address [{clientConnectionRequest.Address}]");


            return (status: false, message: $"Something went wrong, application [{clientConnectionRequest.ApplicationName}] cannot integrate with address [{clientConnectionRequest.Address}] at this moment");

        }

        public (bool status, string message) VerifyClientConnectionByAddressAlias(string applicationName, string addressAlias, ApplicationConnectionTypes applicationType)
        {

            var address = _addressRepository.GetAddressByAlias(addressAlias);
            if (address == null)
                return (status: false, message: $"Address alias [{addressAlias}] doesnot exists");

            return VerifyClientConnection(applicationName, applicationType, address);
        }

        public (bool status, string message) VerifyClientConnectionByAddressName(string applicationName, string addressName, ApplicationConnectionTypes applicationType)
        {
            var address = _addressRepository.GetAddressByName(addressName);
            if (address == null)
                return (status: false, message: $"Address [{addressName}] doesnot exists");

            return VerifyClientConnection(applicationName, applicationType, address);
        }

        private List<ClientConnectionDto> MapClientConnectionsByAddress(Address? address, List<ClientConnection> clientConnections)
        {
            return clientConnections.Select(a => new ClientConnectionDto()
            {
                Id = a.Id,
                Address = address!.Name,
                ApplicationName = _applicationRepository.GetApplication(a.ApplicationId)!.Name,
                ApplicationConnectionType = a.ApplicationConnectionType,
                ConnectedHosts = a.ConnectedHosts,
                FirstConnectionDate = a.FirstConnectionDate,
                IsConnected = a.IsConnected,
                LastConnectionDate = a.LastConnectionDate,
                ProductionInstanceType = a.ProductionInstanceType,
                ConsumptionSettings = a.ConsumptionSettings,
                HostsHistory = a.HostsHistory,
            }).ToList();
        }

        private (bool status, string message) VerifyClientConnection(string applicationName, ApplicationConnectionTypes applicationType, Address? address)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (status: false, message: $"Application [{applicationName}] doesnot exists");


            var clientConnection = _applicationRepository.GetClientConnection(application.Id, application.Id, applicationType);
            if (clientConnection == null)
                return (status: false, message: $"There are not connection between application [{applicationName}] and address [{address!.Name}]");

            if (clientConnection.IsActive != true)
                return (status: false, message: $"Application [{applicationName}] is forbidden to access address [{address!.Name}]");


            return (true, $"Application [{applicationName}] can access address [{address!.Name}]");
        }

        private bool IsReadOrWritePermission(string permission, string address)
        {
            if (permission == "*" || permission == "*:{*}")
                return true;

            var addresses = permission.Split(',');
            if (addresses.Length > 0)
            {
                var addressFound = addresses.Where(a => a == address).FirstOrDefault();
                if (addressFound != null)
                    return true;
            }

            return false;

        }

        public bool RegisterClientHostConnection(string applicationName, string addressName, ApplicationConnectionTypes applicationType, string clientHost, string connectedNode)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return false;

            var address = _addressRepository.GetAddressByName(addressName);
            if (address == null)
                return false;

            var clientConnection = _applicationRepository.GetClientConnection(application.Id, address.Id, applicationType);
            if (clientConnection == null)
                return false;

            // In case ConnectedIPs is null, create new instance.
            clientConnection.ConnectedHosts ??= new List<string>();

            // Update FirstConnectionDate in case this is the first integration happening.
            if (clientConnection.ConnectedHosts.Count() == 0)
                clientConnection.FirstConnectionDate = DateTimeOffset.Now;

            var ipExists = clientConnection.ConnectedHosts!.Exists(x => x == clientHost);

            // add Connection Ip, if the Application is not connected yet.
            if (ipExists != true)
                clientConnection.ConnectedHosts.Add(clientHost);

            if (clientConnection.HostsHistory.ContainsKey(clientHost) != true)
                clientConnection.HostsHistory.Add(clientHost, new ApplicationHost()
                {
                    IsConnected = false,
                    FirstConnectionDate = DateTimeOffset.Now,
                    LastConnectionDate = DateTimeOffset.Now,

                    // node where this client is connected to.
                    ConnectedNode = connectedNode
                });

            // update
            clientConnection.UpdatedAt = DateTimeOffset.Now;
            clientConnection.UpdatedBy = applicationName;

            return _applicationRepository
                .UpdateApplicationAddressConnection(clientConnection);
        }

        public bool UpdateClientConnectionState(string applicationName, string addressName, ApplicationConnectionTypes applicationType, string clientHost, bool isConnected)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return false;

            var address = _addressRepository.GetAddressByName(addressName);
            if (address == null)
                return false;

            var clientConnection = _applicationRepository.GetClientConnection(application.Id, address.Id, applicationType);
            if (clientConnection == null)
                return false;

            clientConnection.IsConnected = isConnected;
            clientConnection.HostsHistory[clientHost].IsConnected = isConnected;

            if (isConnected == true)
            {
                clientConnection.LastConnectionDate = DateTimeOffset.Now;
                clientConnection.HostsHistory[clientHost].LastConnectionDate = DateTimeOffset.Now;
                clientConnection.HostsHistory[clientHost].LastHeartbeatDate = DateTimeOffset.Now;
            }

            // isConnected should be false in all hosts are disconnected
            if (clientConnection.HostsHistory.All(pair => pair.Value.IsConnected != true))
                clientConnection.IsConnected = false;

            clientConnection.UpdatedAt = DateTimeOffset.Now;
            clientConnection.UpdatedBy = applicationName;

            return _applicationRepository
                .UpdateApplicationAddressConnection(clientConnection);
        }

        // This method should be only used for internal purposes. PLEASE, do not expose it via REST Endpoints.
        public ClientConnection? GetClientConnection(string applicationName, string addressName, ApplicationConnectionTypes applicationType)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return null;

            var address = _addressRepository.GetAddressByName(addressName);
            if (address == null)
                return null;

            var clientConnection = _applicationRepository.GetClientConnection(application.Id, address.Id, applicationType);
            if (clientConnection == null)
                return null;

            return clientConnection;
        }

        public List<ClientConnection>? GetConnectedClientConnections()
        {
            return _applicationRepository
                .GetConnectedClientConnections();
        }

        public bool UpdateClientConnection(ClientConnection clientConnection)
        {
            clientConnection.UpdatedAt = DateTimeOffset.Now;

            return _applicationRepository
                .UpdateApplicationAddressConnection(clientConnection);
        }

        public ClientConnection GetClientConnection(Guid connectionId)
        {
            return _applicationRepository
                .GetClientConnection(connectionId)!;
        }
    }
}
