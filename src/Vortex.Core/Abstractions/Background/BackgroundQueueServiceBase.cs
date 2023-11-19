using Vortex.Core.Abstractions.IO.Services;
using Vortex.Core.Utilities.Consts;
using System.Collections.Concurrent;
using System.Threading;

namespace Vortex.Core.Abstractions.Background
{
    public abstract class BackgroundQueueServiceBase<TRequest> : IBackgroundQueueService<TRequest> where TRequest : RequestBase
    {
        private readonly ConcurrentQueue<TRequest> requestQueue;
        private readonly object queueLock;
        private readonly string _prefixFileName;
        private readonly ITemporaryIOService _temporaryIOService;

        private CancellationTokenSource? cancellationTokenSource;


        // Timer for background file check
        private Timer _failedTaskTimer;

        public BackgroundQueueServiceBase(string prefixFileName, ITemporaryIOService temporaryIOService)
        {
            requestQueue = new ConcurrentQueue<TRequest>();

            queueLock = new object();
            _prefixFileName = prefixFileName;
            _temporaryIOService = temporaryIOService;

            var intervalInSec = int.Parse(Environment.GetEnvironmentVariable(EnvironmentConstants.BackgroundServiceFaildTaskInterval)!);
            _failedTaskTimer = new Timer(FailedTaskTimerCallBack, null, TimeSpan.Zero, new TimeSpan(0, 0, intervalInSec));
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

        public void StoreFailedRequest(TRequest request)
        {
            _temporaryIOService.StoreBackgroundTemporaryFile(request, _prefixFileName, Guid.NewGuid());
        }

        private void FailedTaskTimerCallBack(object? state)
        {
            // get logs
            var tempFiles = _temporaryIOService.GetBackgroundFiles(_prefixFileName);
            foreach (var tempFile in tempFiles)
            {
                // re-trying the same request again.
                EnqueueRequest(_temporaryIOService.GetBackgroundTemporaryFileContent<TRequest>(tempFile));
                _temporaryIOService.DeleteFile(tempFile);
            }
        }
    }
}
