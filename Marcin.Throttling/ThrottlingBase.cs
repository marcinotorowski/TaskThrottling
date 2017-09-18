namespace ClassLibrary4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for throttling.
    /// </summary>
    public abstract class ThrottlingBase
    {
        /// <summary>
        /// A queue of tasks to schedule.
        /// </summary>
        protected readonly IList<Func<CancellationToken, Task>> AllTasks = new List<Func<CancellationToken, Task>>();

        /// <summary>
        /// Adds a given task to the queue.
        /// </summary>
        /// <param name="task">The task creator.</param>
        public void Add(Func<CancellationToken, Task> task)
        {
            if (task == null)
            {
                // ReSharper disable once UseNameofExpression
                throw new ArgumentNullException("task");
            }

            this.AllTasks.Add(task);
        }

        /// <summary>
        /// Adds a given task to the queue.
        /// </summary>
        /// <param name="throttling">The throttled task.</param>
        public void Add(ThrottlingBase throttling)
        {
            if (throttling == null)
            {
                // ReSharper disable once UseNameofExpression
                throw new ArgumentNullException("throttling");
            }

            this.AllTasks.Add(throttling.ExecuteAsync);
        }

        /// <summary>
        /// Adds given tasks to the queue.
        /// </summary>
        /// <param name="tasks">The task creators.</param>
        public void Add(params Func<CancellationToken, Task>[] tasks)
        {
            if (tasks == null)
            {
                // ReSharper disable once UseNameofExpression
                throw new ArgumentNullException("tasks");
            }

            this.Add((IEnumerable<Func<CancellationToken, Task>>)tasks);
        }

        /// <summary>
        /// Adds given tasks to the queue.
        /// </summary>
        /// <param name="tasks">The task creators.</param>
        public void Add(params ThrottlingBase[] tasks)
        {
            if (tasks == null)
            {
                // ReSharper disable once UseNameofExpression
                throw new ArgumentNullException("tasks");
            }

            this.Add((IEnumerable<ThrottlingBase>)tasks);
        }

        /// <summary>
        /// Adds given tasks to the queue.
        /// </summary>
        /// <param name="tasks">The task creators.</param>
        public void Add(IEnumerable<Func<CancellationToken, Task>> tasks)
        {
            if (tasks == null)
            {
                // ReSharper disable once UseNameofExpression
                throw new ArgumentNullException("tasks");
            }

            foreach (var item in tasks.Where(x => x != null))
            {
                this.AllTasks.Add(item);
            }
        }

        /// <summary>
        /// Adds given tasks to the queue.
        /// </summary>
        /// <param name="tasks">The task creators.</param>
        public void Add(IEnumerable<ThrottlingBase> tasks)
        {
            if (tasks == null)
            {
                // ReSharper disable once UseNameofExpression
                throw new ArgumentNullException("tasks");
            }

            foreach (var item in tasks.Where(x => x != null))
            {
                this.AllTasks.Add(item.ExecuteAsync);
            }
        }

        /// <summary>
        /// Executes the tasks asynchronously.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that executes subtasks.</returns>
        public abstract Task ExecuteAsync(CancellationToken token);
    }
}