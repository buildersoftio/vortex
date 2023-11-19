namespace Vortex.Core.Abstractions.Background
{
    public interface IBackgroundQueueService<TRequest> where TRequest : RequestBase
    {
        void EnqueueRequest(TRequest request);
        void StoreFailedRequest(TRequest request);
        void Handle(TRequest request);
    }
}
