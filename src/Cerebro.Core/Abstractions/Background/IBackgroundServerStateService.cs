namespace Cerebro.Core.Abstractions.Background
{
    public interface IBackgroundServerStateService<TRequest>
    {
        void EnqueueRequest(TRequest request);
        void Handle(TRequest request);
    }
}
