using Newtonsoft.Json.Bson;

namespace Cerebro.Core.Abstractions.Background
{
    public abstract class TimedBackgroundServiceBase : ITimedBackgroundService
    {
        private readonly TimeSpan _period;
        private Timer _timer;

        public TimedBackgroundServiceBase(TimeSpan period)
        {
            _period = period;
            _timer = new Timer(TimerCallBack, null, TimeSpan.Zero, period);
        }

        private void TimerCallBack(object? state)
        {
            OnTimer_Callback(state);
        }

        public abstract void OnTimer_Callback(object? state);
    }
}
