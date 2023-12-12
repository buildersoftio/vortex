using System.Collections.Concurrent;
using System.Threading;

namespace Vortex.Core.Abstractions.Background
{
    public abstract class ParallelBackgroundQueueServiceBase<TRequest> : IParallelBackgroundQueueService<TRequest>
    {

        private readonly ConcurrentQueue<TRequest> requestQueue;
        private readonly object queueLock;
        private CancellationTokenSource? cancellationTokenSource;
        private readonly int maxConcurrentTasks;
        private readonly SemaphoreSlim semaphore;

        // background timer
        private readonly TimeSpan? _period;
        private Timer _timer;


        public ParallelBackgroundQueueServiceBase(int maxConcurrentTasks = 1, TimeSpan? period = null)
        {
            requestQueue = new ConcurrentQueue<TRequest>();
            queueLock = new object();
            this.maxConcurrentTasks = maxConcurrentTasks;
            semaphore = new SemaphoreSlim(maxConcurrentTasks);

            _period = period;
        }

        public void EnqueueRequest(TRequest request)
        {
            requestQueue.Enqueue(request);
            // Start the processing if it was not already running.
            if (cancellationTokenSource == null || cancellationTokenSource.IsCancellationRequested)
            {
                StartProcessing();
            }
        }

        public void StartProcessing()
        {
            cancellationTokenSource = new CancellationTokenSource();

            // TODO: RESEARCH: should we run ProcessRequests in another thread, or in main thread?
            Task.Run(() => ProcessRequests());
        }

        public void StopProcessing()
        {
            cancellationTokenSource?.Cancel();
        }

        private void ProcessRequests()
        {
            cancellationTokenSource = new CancellationTokenSource();
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                if (!requestQueue.IsEmpty)
                {
                    semaphore.Wait();
                    if (requestQueue.TryDequeue(out TRequest? request))
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                Handle(request);
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        });
                    }
                }
            }
        }

        public abstract void Handle(TRequest request);

        public void StartTimer()
        {
            if (_period != null)
                _timer = new Timer(TimerCallBack, null, TimeSpan.Zero, _period.Value);
        }

        private void TimerCallBack(object? state)
        {
            OnTimer_Callback(state);
        }

        public abstract void OnTimer_Callback(object? state);
    }
}
