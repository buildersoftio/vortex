using Microsoft.Extensions.Logging;
using Murmur;
using System.Collections.Concurrent;
using Vortex.Core.Abstractions.Services.Data;
using Vortex.Core.Abstractions.Services.Orchestrations;
using Vortex.Core.Models.Containers;
using Vortex.Core.Models.Data;
using Vortex.Core.Repositories;

namespace Vortex.Core.Services.Routing
{
    public class DataDistributionService : IDataDistributionService
    {
        private readonly ILogger<DataDistributionService> _logger;
        private readonly IServerCoreStateManager _serverCoreStateManager;
        private readonly IAddressRepository _addressRepository;

        private readonly ConcurrentDictionary<string, int> _addressesCached;


        // Is used for messageId hashing to distribute data into different partitions.
        private readonly Murmur32 _murmurHash;

        public DataDistributionService(ILogger<DataDistributionService> logger,
            IServerCoreStateManager serverCoreStateManager,
            IAddressRepository addressRepository)
        {
            _logger = logger;
            _serverCoreStateManager = serverCoreStateManager;
            _addressRepository = addressRepository;

            _addressesCached = new ConcurrentDictionary<string, int>();

            _murmurHash = MurmurHash.Create32(seed: 12345);
        }


        public (bool success, int? partitionKey, string message) Distribute(string address, PartitionMessage message)
        {
            // cache the address
            int addressId = GetAddressFromCache(address);
            if (addressId == -1)
                return (success: false, partitionKey: -1, message: "The given address name doesnot exists");

            var addressContainer = _serverCoreStateManager.GetAddressContainer(addressId);

            // Find the partition_id where the message will be stored into PartitionQueue
            if (message.MessageId == null)
            {
                if (message.PartitionIndex == null || message.PartitionIndex == -1)
                    message.PartitionIndex = GetNextPartitionIndex(addressContainer);
            }
            else
            {
                if (message.PartitionIndex == null || message.PartitionIndex == -1)
                    message.PartitionIndex = GetHashedPartitionIndex(message.MessageId, addressContainer.PartitionEntries!.Count());
            }

            addressContainer.PartitionDataProcessors[message.PartitionIndex.Value].EnqueueRequest(message);

            return (success: true, partitionKey: message.PartitionIndex, message: "Message stored in-memory");
        }


        public (bool success, int? partitionKey, string message) Distribute(string address, Span<PartitionMessage> message)
        {
            // cache the address
            int addressId = GetAddressFromCache(address);
            if (addressId != -1)
                return Distribute(addressId, message);

            return (success: false, partitionKey: -1, message: "The given address name doesnot exists");
        }

        private int GetHashedPartitionIndex(byte[] messageId, int partitionCount)
        {
            if (partitionCount == 1)
                return 0;

            byte[] hash = _murmurHash.ComputeHash(messageId);
            int hashValue = BitConverter.ToInt32(hash, 0);

            return Math.Abs(hashValue % partitionCount);
        }

        private int GetAddressFromCache(string address)
        {
            //TODO: Try to remove addresses if address is deleted from the system

            if (_addressesCached.ContainsKey(address))
                return _addressesCached[address];

            var addressDetails = _addressRepository.GetAddressByName(address);
            if (addressDetails == null)
                return -1;

            // cache this address for other messages
            _addressesCached.TryAdd(address, addressDetails.Id);

            return addressDetails.Id;
        }

        private int GetNextPartitionIndex(AddressContainer addressContainer)
        {
            addressContainer.CurrentPartitionId++;
            if (addressContainer.CurrentPartitionId == addressContainer.PartitionEntries!.Count())
                addressContainer.CurrentPartitionId = 0;

            return addressContainer.CurrentPartitionId;
        }

        public (bool success, int? partitionKey, string message) Distribute(int addressId, Span<PartitionMessage> message)
        {
            var addressContainer = _serverCoreStateManager.GetAddressContainer(addressId);

            // Find the partition_id where the message will be stored into PartitionQueue
            if (message[0].MessageId == null)
            {
                if (message[0].PartitionIndex == null || message[0].PartitionIndex == -1)
                    message[0].PartitionIndex = GetNextPartitionIndex(addressContainer);
            }
            else
            {
                if (message[0].PartitionIndex == null || message[0].PartitionIndex == -1)
                    message[0].PartitionIndex = GetHashedPartitionIndex(message[0].MessageId!, addressContainer.PartitionEntries!.Count());
            }

            addressContainer.PartitionDataProcessors[message[0].PartitionIndex!.Value].EnqueueRequest(message[0]);

            return (success: true, partitionKey: message[0].PartitionIndex, message: "Message stored in-memory");
        }
    }
}
