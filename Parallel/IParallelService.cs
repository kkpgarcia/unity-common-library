namespace Common.Parallel
{
    public interface IParallelService
    {
        void OnApplicationQuit();
        void Start();
        void Stop();
        void QueueTask(JobHandler handler);
    }
}
