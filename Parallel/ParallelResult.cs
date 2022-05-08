using Common.Reactive;

namespace Common.Parallel
{
    public class ParallelResult
    {
        public enum ParallelStateEnum
        {
            UNPROCESSED,
            STARTED,
            PROCESSING,
            COMPLETED,
            ABORTED
        }

        private object _result;

        public object Value
        {
            get => _result;
        }

        public Observable<ParallelStateEnum> State;

        public ParallelResult()
        {
	        this.State = new Observable<ParallelStateEnum>(ParallelStateEnum.UNPROCESSED);
        }

        public void SetState(ParallelStateEnum state)
        {
            this.State.Value = state;
        }

        public void SetResultObject(object result)
        {
            this._result = result;
        }
    }
}
