using System.Threading.Tasks;
using Common.Reactive;
using UnityEngine;

namespace Common.Parallel
{
    public class ParallelServiceTest : MonoBehaviour
    {
        private JobHandler _threadHandler;
        private JobHandler _taskHandler;
        public void Start()
        {
            DebugParallelJob threadJob = new DebugParallelJob();
            _threadHandler = threadJob.Schedule();

            AnotherDebugParallelJob taskJob = new AnotherDebugParallelJob();
            _taskHandler = taskJob.Schedule();

            IObservableStream<ParallelResult.ParallelStateEnum> streamA = _threadHandler.Result.State.GetDataStream();
            streamA.Subscribe((s, a) =>
            {
	            Debug.Log("Thread Job is completed with the result of: " + _threadHandler.Result.Value);
            });

            IObservableStream<ParallelResult.ParallelStateEnum> streamB = _taskHandler.Result.State.GetDataStream();
            streamB.Subscribe((s, a) =>
            {
	            Debug.Log("Thread Job is completed with the result of: " + _taskHandler.Result.Value);
            });


            // StartCoroutine(CheckResult());
            // StartCoroutine(TaskCheckResult());
        }

        // IEnumerator CheckResult()
        // {
        //     while (!_threadHandler.IsCompleted)
        //     {
        //         Debug.Log("Waiting for job to be completed...");
        //         yield return null;
        //     }
        //
        //     if (_threadHandler.IsCompleted)
        //     {
        //         Debug.Log("Thread Job is completed with the result of: " + _threadHandler.Result.Value);
        //     }
        // }

        // IEnumerator TaskCheckResult()
        // {
        //     while (!_taskHandler.IsCompleted)
        //     {
        //         Debug.Log("Waiting for job to be completed...");
        //         yield return null;
        //     }
        //
        //     if (_taskHandler.IsCompleted)
        //     {
        //         Debug.Log("Task Job is completed with the result of: " + _taskHandler.Result.Value);
        //     }
        // }
    }

    public class DebugParallelJob : ThreadParallelJob
    {
        public override void OnStart()
        {
            base.OnStart();
            Debug.Log("Thread: " + this.GetResult().State);
        }

        public override void Execute(Task task = null)
        {
            base.Execute(task);
            Debug.Log("Thread: " + this.GetResult().State);
        }

        public override void OnComplete(Task task = null)
        {
            base.OnComplete(task);
            Debug.Log("Thread: " + this.GetResult().State);

            this.GetResult().SetResultObject("Hello World from Thread Parallel Service!");
        }
    }

    public class AnotherDebugParallelJob : TaskParallelJob
    {
        public override void OnStart()
        {
            base.OnStart();
            Debug.Log("Task: " + this.GetResult().State);
        }

        public override void Execute(Task task = null)
        {
            base.Execute(task);
            Debug.Log("Task: " + this.GetResult().State);
        }

        public override void OnComplete(Task task = null)
        {
            base.OnComplete(task);
            Debug.Log("Task: " + this.GetResult().State);

            this.GetResult().SetResultObject("Hello World from Task Parallel Service!");
        }
    }
}
