namespace Marcin.Throttling.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ClassLibrary4;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for throttling.
    /// </summary>
    [TestClass]
    public class ThrottingTests
    {
        /// <summary>
        /// Tests basic throttling.
        /// </summary>
        [TestMethod]
        public void TestThrottling()
        {
            ThrottlingBase machine1 = new ThrottleSequential();
            ThrottlingBase machine2 = new ThrottleSequential();
            ThrottlingBase machine3 = new ThrottleSequential();

            var lockObject = new object();
            var results = new List<string>();

            Action<string> write = s =>
            {
                lock (lockObject)
                {
                    results.Add(s);
                }
            };

            machine1.Add(token => Task.Run(() => { write("pkg 01 / machine 01"); }, token));
            machine1.Add(token => Task.Run(() => { write("pkg 02 / machine 01"); }, token));
            machine1.Add(token => Task.Run(() => { write("pkg 03 / machine 01"); }, token));
            machine1.Add(token => Task.Run(() => { write("pkg 04 / machine 01"); }, token));
            machine1.Add(token => Task.Run(() => { write("pkg 05 / machine 01"); }, token));
                                                   
            machine2.Add(token => Task.Run(() => { write("pkg 06 / machine 02"); }, token));
            machine2.Add(token => Task.Run(() => { write("pkg 07 / machine 02"); }, token));
            machine2.Add(token => Task.Run(() => { write("pkg 08 / machine 02"); }, token));
                                                   
            machine3.Add(token => Task.Run(() => { write("pkg 09 / machine 03"); }, token));
            machine3.Add(token => Task.Run(() => { write("pkg 10 / machine 03"); }, token));
            machine3.Add(token => Task.Run(() => { write("pkg 11 / machine 03"); }, token));
            machine3.Add(token => Task.Run(() => { write("pkg 12 / machine 03"); }, token));
            machine3.Add(token => Task.Run(() => { write("pkg 13 / machine 03"); }, token));
            machine3.Add(token => Task.Run(() => { write("pkg 14 / machine 03"); }, token));
            machine3.Add(token => Task.Run(() => { write("pkg 15 / machine 03"); }, token));

            ThrottlingBase together = new ThrottleParallel(2);
            together.Add(machine1);
            together.Add(machine2);
            together.Add(machine3);

            together.ExecuteAsync(default(CancellationToken)).Wait(default(CancellationToken));

            var firstMachine1 = results.IndexOf(results.First(item => item.Contains("machine 01")));
            var firstMachine2 = results.IndexOf(results.First(item => item.Contains("machine 02")));
            var firstMachine3 = results.IndexOf(results.First(item => item.Contains("machine 03")));
            var lastMachine1 = results.IndexOf(results.Last(item => item.Contains("machine 01")));
            var lastMachine2 = results.IndexOf(results.Last(item => item.Contains("machine 02")));
            var lastMachine3 = results.IndexOf(results.Last(item => item.Contains("machine 03")));

            // There should be 15 tasks that finished
            Assert.AreEqual(15, results.Count);

            // Root tasks had several subitems: they must be executed in order (so that first subitem finished before the last one):
            Assert.IsTrue(firstMachine1 < lastMachine1);
            Assert.IsTrue(firstMachine2 < lastMachine2);
            Assert.IsTrue(firstMachine3 < lastMachine3);

            // First root task must be started before second root task
            Assert.IsTrue(firstMachine1 < firstMachine2);

            // Second root task must be started before third root task
            Assert.IsTrue(firstMachine2 < firstMachine3);

            // Because we used throttling of 2, the first task from 3rd root must be executed after first root one.
            Assert.IsTrue(firstMachine1 < firstMachine3);

            // Because we used throttling of 2, the first task from 3rd root must be executed after second root one.
            Assert.IsTrue(firstMachine2 < firstMachine3);

            // Finally, whichever finished earlier (first or second) it must be before the first element of third task.
            Assert.IsTrue(Math.Min(lastMachine1, lastMachine2) < firstMachine3);
        }
    }
}
