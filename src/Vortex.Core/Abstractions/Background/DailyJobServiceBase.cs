namespace Vortex.Core.Abstractions.Background
{
    public abstract class DailyJobServiceBase : IDailyJobService
    {
        private Timer _dailyTimer;

        public DailyJobServiceBase(DateTime dateToTrigger)
        {
            // Calculate time to run the daily task at 8:00 AM
            DateTime now = DateTime.Now;
            if (now >= dateToTrigger)
                dateToTrigger = dateToTrigger.AddDays(1);

            TimeSpan timeToNextTime = dateToTrigger - now;
            _dailyTimer = new Timer(OnDailyTimerCallback, null, timeToNextTime, TimeSpan.FromDays(1));

        }

        private void OnDailyTimerCallback(object? state)
        {
            OnDailyJob_Callback(state);
        }

        public abstract void OnDailyJob_Callback(object? state);
    }
}
