using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Common.Parallel
{
    public class TaskParallelService : IParallelService//, ISystemInitializable, ISystemOnRegistered
    {
        private readonly BlockingCollection<IParallelJob> _queue;
        private CancellationTokenSource _cancellationTokenSource;
        private BlockingCollection<Thread> _activeThreads;
        private BlockingCollection<Thread> _inactiveThreads;

        private const int MAX_WORKER_COUNT = 4;

        private bool _isInitialized;
        public bool IsInitialized { get => _isInitialized; }
        public void Initialize()
        {
	        int minWorker, minIOC;
	        ThreadPool.GetMinThreads(out minWorker, out minIOC);
	        ThreadPool.SetMinThreads(MAX_WORKER_COUNT, minIOC);

	        this._isInitialized = true;
        }

        public void OnRegistered()
        {
	        Start();
        }

        public TaskParallelService()
        {
            _queue = new BlockingCollection<IParallelJob>();
            _cancellationTokenSource = new CancellationTokenSource();

            Application.quitting += OnApplicationQuit;
        }

        public void QueueTask(JobHandler handler)
        {
            if (handler.GetJob() == null)
            {
                throw new ArgumentNullException();
            }

            _queue.Add(handler.GetJob());
        }

        public void OnApplicationQuit()
        {
            Stop();
        }

        public void Start()
        {
	        Task.Run(MonitorAsync, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task MonitorAsync()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                foreach (IParallelJob job in _queue.GetConsumingEnumerable())
                {
                    await Task.Run(() => Task.FromResult(new Worker().Work(job, _cancellationTokenSource.Token)),
                        _cancellationTokenSource.Token);
                }
            }
        }
    }
}
