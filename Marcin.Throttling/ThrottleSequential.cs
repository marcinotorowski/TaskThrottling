namespace Marcin.Throttling
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A throttle manager.
    /// </summary>
    public class ThrottleSequential : ThrottlingBase
    {
        /// <summary>
        /// Executes all tasks.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that executes all subtasks.</returns>
        public override async Task ExecuteAsync(CancellationToken token)
        {
            foreach (var task in this.AllTasks.ToArray())
            {
                await task(token).ConfigureAwait(false);
            }
        }
    }
}
