using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SampleTest
{
    public class TestContainer {
        public ManualResetEvent WaitHandle;
        public List<int> TestNumbers;

        public TestContainer(ManualResetEvent manualResetEvent) {
            WaitHandle = manualResetEvent;
            TestNumbers = new List<int>();
        }
    };

    [TestClass]
    public class UnitTestThreadPool {
        [TestMethod]
        public void Test0CallTheadPool() {
            var manualResetEvent = new ManualResetEvent(false);
            var container = new TestContainer(manualResetEvent);
            
            ThreadPool.QueueUserWorkItem(ThreadEntryPoint, container);

            manualResetEvent.WaitOne();

            Assert.AreEqual(container.TestNumbers.Count, 10);

            for (int i = 0; i < 10; i++) {
                Assert.AreEqual(container.TestNumbers[i], i);
            }
        }

        protected static void ThreadEntryPoint(object stateInfo)
        {
            for (int i = 0; i < 10; i++) {
                var container = (TestContainer)stateInfo;
                container.TestNumbers.Add(i);
                Thread.Sleep(1000);

                if (container.TestNumbers.Count >= 10) container.WaitHandle.Set();
            }
        }
    }
}
