using System;
using UnityEngine.Networking;

namespace Common.Parallel
{
    public class JobHandler
    {
        private readonly string _id;
        public string  ID
        {
            get => _id;
        }

        public bool IsCompleted
        {
            get => this.Result.State.Value == ParallelResult.ParallelStateEnum.COMPLETED;
        }

        public ParallelResult Result
        {
            get => this._job.GetResult();
        }

        private IParallelJob _job;

        public JobHandler(IParallelJob job)
        {
            this._id = job.ID;
            this._job = job;
        }

        public IParallelJob GetJob()
        {
            return this._job;
        }
    }
}
