using Cerebro.Core.Abstractions.Clustering;

namespace Cerebro.Grpc.Servers
{
    public class gRPCClientIntegrationServer : INodeExchangeServer
    {
        public gRPCClientIntegrationServer()
        {

        }

        public Task ShutdownAsync()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
