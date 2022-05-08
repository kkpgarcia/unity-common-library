using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Parallel
{
    public class Worker
    {
        CancellationTokenSource internalTokenSource = new CancellationTokenSource();
        CancellationToken internalToken;

        public async Task Work(IParallelJob job, CancellationToken externalToken)
        {
            this.internalToken = internalTokenSource.Token;

            using (CancellationTokenSource linkedCts =
                   CancellationTokenSource.CreateLinkedTokenSource(internalToken, externalToken))
            {
                try
                {
                    await Task.Factory
                        .StartNew(job.OnStart, linkedCts.Token)
                        .ContinueWith(job.Execute, linkedCts.Token)
                        .ContinueWith(job.OnComplete, linkedCts.Token);
                }
                catch (OperationCanceledException) {
                    if (internalToken.IsCancellationRequested) {
                        Console.WriteLine("Operation has been canceled");
                    }
                    else if (externalToken.IsCancellationRequested) {
                        Console.WriteLine("Cancelling per user request.");
                        externalToken.ThrowIfCancellationRequested();
                    }
                }
            }
        }
    }
}
