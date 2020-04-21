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

    public class TestCompoundContainer {
        public ManualResetEvent WaitHandle;
        public int[] TestNumbers;
        public int Index;

        public TestCompoundContainer(ManualResetEvent manualResetEvent, int[] testNumbers, int index) {
            WaitHandle = manualResetEvent;
            TestNumbers = testNumbers;
            Index = index;
        }
    }

    [TestClass]
    public class UnitTestThreadPool {
        [TestMethod]
        public void Test0CallSingleTheadPool() {
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

        [TestMethod]
        public void Test1CallMultiThreadPoolNonCriticalSection() {
            var threadCount = 3;
            var resetEvents = GenerateResetEvents(threadCount);
            var data = new int[threadCount];

            var containers = new TestCompoundContainer[threadCount];
            for (int i = 0; i < threadCount; i++) {
                containers[i] = new TestCompoundContainer(resetEvents[i], data, i);

                ThreadPool.QueueUserWorkItem(ThreadEntryPointCompound, containers[i]);
            }

            WaitHandle.WaitAll(resetEvents);

            for (int i = 0; i < threadCount; i++) Assert.AreEqual(data[i], 10);
        }

        protected ManualResetEvent[] GenerateResetEvents(int threadCount) {
            var resetEvents = new ManualResetEvent[threadCount];
            for (int i = 0; i < resetEvents.Length; i++) resetEvents[i] = new ManualResetEvent(false);

            return resetEvents;
        }

        protected static void ThreadEntryPointCompound(object stateInfo) {
            var container = (TestCompoundContainer)stateInfo;

            for (int i = 0; i < 10; i++) {
                container.TestNumbers[container.Index]++;
                Thread.Sleep(100);                
            }

            container.WaitHandle.Set();
        }
    }
}
