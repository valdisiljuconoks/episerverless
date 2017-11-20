using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Models
{
    /// <summary>
    /// Credits:
    /// http://stackoverflow.com/questions/9343594/how-to-call-asynchronous-method-from-synchronous-method-in-c
    /// </summary>
    public static class AsyncHelper
    {
        private static readonly TaskFactory MyTaskFactory =
            new TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return MyTaskFactory.StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            MyTaskFactory.StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }
    }
}