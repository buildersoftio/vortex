namespace Vortex.Core.Abstractions.Background
{
    public interface ISimpleBackgroundQueueService<TRequest>
    {
        void EnqueueRequest(TRequest request);
        void Handle(TRequest request);
    }
}
