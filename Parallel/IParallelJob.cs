using System;
using System.Threading.Tasks;

namespace Common.Parallel
{
    public interface IParallelJob
    {
        string ID { get; }
        JobHandler Schedule();
        void OnStart();
        void Execute(Task task = null);
        void OnComplete(Task task = null);
        ParallelResult GetResult();
    }

    public abstract class ParallelJob : IParallelJob
    {
        private readonly string _id;
        public string  ID
        {
            get => _id;
        }

        protected ParallelResult _result;

        protected abstract IParallelService Service
        {
            get;
        }

        public ParallelJob()
        {
            this._id = Guid.NewGuid().ToString();
            this._result = new ParallelResult();
        }

        public JobHandler Schedule()
        {
            this._result.SetState(ParallelResult.ParallelStateEnum.UNPROCESSED);

            JobHandler handler = new JobHandler(this);

            Service.QueueTask(handler);

            return handler;
        }

        public virtual void OnStart()
        {
            this._result.SetState(ParallelResult.ParallelStateEnum.STARTED);
        }

        public  virtual void Execute(Task task = null)
        {
            this._result.SetState(ParallelResult.ParallelStateEnum.PROCESSING);
        }

        public  virtual void OnComplete(Task task = null)
        {
            this._result.SetState(ParallelResult.ParallelStateEnum.COMPLETED);
        }

        public virtual void Cancel()
        {
            this._result.SetState(ParallelResult.ParallelStateEnum.ABORTED);
        }

        public ParallelResult GetResult()
        {
            return _result;
        }
    }

    public class ThreadParallelJob : ParallelJob
    {
        protected override IParallelService Service => null; //ServiceLocator.GetService<ThreadParallelService>();
    }

    public class TaskParallelJob : ParallelJob
    {
        protected override IParallelService Service => null; //ServiceLocator.GetService<TaskParallelService>();
    }
}
