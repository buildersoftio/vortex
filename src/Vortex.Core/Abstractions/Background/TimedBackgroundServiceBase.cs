namespace Cerebro.Core.Abstractions.Background
{
    public abstract class TimedBackgroundServiceBase<HeartbeatTimerRequest> : ITimedBackgroundService<HeartbeatTimerRequest>
    {
        private readonly TimeSpan _period;
        private Timer _timer;

        public TimedBackgroundServiceBase(TimeSpan period)
        {
            _period = period;
        }

        private void TimerCallBack(object? state)
        {
            OnTimer_Callback(state);
        }

        public abstract void OnTimer_Callback(object? state);

        public void Start()
        {
            _timer = new Timer(TimerCallBack, null, TimeSpan.Zero, _period);
        }
    }
}
