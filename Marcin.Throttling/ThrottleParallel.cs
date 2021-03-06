namespace Marcin.Throttling
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A task throttle that runs tasks in parallel, using no more than a given number of tasks.
    /// </summary>
    public class ThrottleParallel : ThrottlingBase
    {
        /// <summary>
        /// Maximum number of parallel tasks.
        /// </summary>
        private readonly int maximumParallel;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottleParallel"/> class.
        /// </summary>
        /// <param name="maximumParallel">Maximum number of tasks running parallel.</param>
        public ThrottleParallel(int maximumParallel)
        {
            this.maximumParallel = maximumParallel;
        }

        /// <summary>
        /// Executes the tasks asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that executes subtasks.</returns>
        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var myTasks = this.AllTasks.ToArray();

            using (var semaphore = new SemaphoreSlim(this.maximumParallel, this.maximumParallel))
            {
                await Task.WhenAll(myTasks.Select(
                    async t =>
                        {
                            try
                            {
                                // ReSharper disable once AccessToDisposedClosure
                                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                                await t(cancellationToken).ConfigureAwait(false);
                            }
                            finally
                            {
                                // ReSharper disable once AccessToDisposedClosure
                                semaphore.Release(1);
                            }
                        }));
            }   
        }
    }
}