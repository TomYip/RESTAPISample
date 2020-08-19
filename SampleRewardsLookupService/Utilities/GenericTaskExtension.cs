namespace SampleRewardsLookupService.Utilities
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class GenericTaskExtension
    {
        /// <summary>
        /// (Extension) Method to throw timeout exception when a task takes longer than the given time to complete.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task">The class to add the extension method.</param>
        /// <param name="timeout">Time allowed to complete the task.</param>
        /// <returns>A Task<TResult> object if the task comples within given time. Otherwise, throws timeout exception.</returns>
        /// <remarks>
        /// Credits to Lawrence Johnston and other engineers contributed to his solution on StackOverflow:
        /// https://stackoverflow.com/questions/4238345/asynchronously-wait-for-taskt-to-complete-with-timeout
        /// </remarks>
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task;  // Very important in order to propagate exceptions
                }
                else
                {
                    throw new TimeoutException();
                }
            }
        }
    }
}