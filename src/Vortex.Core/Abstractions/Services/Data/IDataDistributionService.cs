using Vortex.Core.Models.Data;

namespace Vortex.Core.Abstractions.Services.Data
{
    public interface IDataDistributionService
    {
        (bool success, int? partitionKey, string message) Distribute(string address, PartitionMessage message);
        (bool success, int? partitionKey, string message) Distribute(string address, Span<PartitionMessage> message);
        (bool success, int? partitionKey, string message) Distribute(int addressId, Span<PartitionMessage> message);
    }
}
