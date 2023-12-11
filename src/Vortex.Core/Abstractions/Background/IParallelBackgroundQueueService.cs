namespace Vortex.Core.Abstractions.Background
{
    public interface IParallelBackgroundQueueService<TRequest>
    {
        void EnqueueRequest(TRequest request);
        void Handle(TRequest request);

        void StartTimer();
    }
}
