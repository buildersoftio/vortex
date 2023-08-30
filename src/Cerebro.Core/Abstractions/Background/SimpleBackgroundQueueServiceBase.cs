using System.Collections.Concurrent;

namespace Cerebro.Core.Abstractions.Background
{
    public abstract class SimpleBackgroundQueueServiceBase<TRequest> : ISimpleBackgroundQueueService<TRequest>
    {
        private readonly ConcurrentQueue<TRequest> requestQueue;
        private readonly object queueLock;

        private CancellationTokenSource? cancellationTokenSource;

        public SimpleBackgroundQueueServiceBase()
        {
            requestQueue = new ConcurrentQueue<TRequest>();
            queueLock = new object();
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
            Task.Run(() => ProcessRequests());
        }

        public void StopProcessing()
        {
            cancellationTokenSource?.Cancel();
        }

        private void ProcessRequests()
        {
            while (!cancellationTokenSource!.IsCancellationRequested)
            {
                if (requestQueue.IsEmpty)
                {
                    // Wait for a short time to check for new requests.
                    // If no new requests arrive within this duration, the background thread will stop.
                    if (!WaitForNewRequests(5000)) // Adjust the waiting duration as needed.
                    {
                        StopProcessing();
                        return;
                    }
                }

                TRequest? request;
                lock (queueLock)
                {
                    if (requestQueue.TryDequeue(out request))
                    {
                        // Simulate some processing delay (you can replace this with actual processing logic).
                        if (request != null)
                            Handle(request);
                    }
                }
            }
        }

        public abstract void Handle(TRequest request);

        private bool WaitForNewRequests(int millisecondsTimeout)
        {
            lock (queueLock)
            {
                if (requestQueue.IsEmpty)
                {
                    Monitor.Wait(queueLock, millisecondsTimeout);
                }
                return !requestQueue.IsEmpty;
            }
        }
    }
}
