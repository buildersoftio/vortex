namespace Cerebro.Core.Abstractions.Background
{
    public interface ITimedBackgroundService<TType>
    {
        void OnTimer_Callback(object state);
        void Start();
    }
}
