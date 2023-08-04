namespace Cerebro.Core.Abstractions.Background
{
    public interface IBackgroundQueueService<TRequest>
    {
        void EnqueueRequest(TRequest request);
        void Handle(TRequest request);
    }
}
