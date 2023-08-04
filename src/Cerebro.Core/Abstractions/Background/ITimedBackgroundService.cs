namespace Cerebro.Core.Abstractions.Background
{
    public interface ITimedBackgroundService
    {
        void OnTimer_Callback(object state);
    }
}
