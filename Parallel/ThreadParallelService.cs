using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Common.Parallel
{
    public class ThreadParallelService : IParallelService//, ISystemInitializable
    {
        private readonly LinkedList<Thread> _workers;
        private readonly LinkedList<IParallelJob> _jobQueue;

        private bool _disallowAdd;
        private bool _disposed;

        private const int MAX_WORKER_COUNT = 4;

        public bool IsInitialized { get => _workers?.Count == MAX_WORKER_COUNT; }
        public void Initialize()
        {
	        for (int i = 0; i < MAX_WORKER_COUNT; i++)
	        {
		        Thread worker = new Thread(this.Worker) {Name = "Worker " + i};
		        worker.Start();
		        this._workers.AddLast(worker);
	        }
        }

        public ThreadParallelService()
        {
            this._workers = new LinkedList<Thread>();
            this._jobQueue = new LinkedList<IParallelJob>();

            Application.quitting += OnApplicationQuit;
        }

        public void OnApplicationQuit()
        {
            Stop();
        }

        public void Start()
        {
        }

        public void Stop()
        {
            bool waitForThreads = false;
            lock (this._jobQueue)
            {
                if (!this._disposed)
                {
                    this._disallowAdd = true;
                    this._jobQueue.Clear();

                    this._disposed = true;
                    Monitor.PulseAll(this._jobQueue);
                    waitForThreads = true;
                }
            }

            if (waitForThreads)
            {
                foreach (Thread worker in this._workers)
                {
                    worker.Abort();
                }
            }
        }

        public void QueueTask(JobHandler handler)
        {
            lock (this._jobQueue)
            {
                if (this._disallowAdd) { throw new InvalidOperationException("This Pool instance is in the process of being disposed, can't add anymore"); }
                if (this._disposed) { throw new ObjectDisposedException("This Pool instance has already been disposed"); }

                this._jobQueue.AddLast(handler.GetJob());

                Monitor.PulseAll(this._jobQueue);
            }
        }

        private void Worker()
        {
            try
            {
                IParallelJob task;
                while (true)
                {
                    Debug.Log("Looking for work...");
                    lock (this._jobQueue)
                    {
                        while (true)
                        {
                            if (this._disposed)
                            {
                                return;
                            }

                            if (null != this._workers.First &&
                                object.ReferenceEquals(Thread.CurrentThread, this._workers.First.Value) &&
                                this._jobQueue.Count > 0)
                            {
                                task = this._jobQueue.First.Value;
                                this._jobQueue.RemoveFirst();
                                this._workers.RemoveFirst();
                                Monitor.PulseAll(this._jobQueue);
                                break;
                            }

                            Monitor.Wait(this._jobQueue);
                        }

                        this._workers.AddLast(Thread.CurrentThread);
                    }

                    task.OnStart();
                    task.Execute();
                    task.OnComplete();
                }
            }
            catch (ThreadAbortException e)
            {
                Debug.LogWarning(Thread.CurrentThread.Name + " is aborting");
            }
        }
    }
}
