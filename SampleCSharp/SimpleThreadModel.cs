using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

/*
TODO
------
cancellation token? 


*/



namespace SampleCSharp
{
    public class ThreadHandler {
        public static void Handle(object state) {
            Console.WriteLine($"ThreadId={Thread.CurrentThread.ManagedThreadId}");
        }
    }

    public class QueueData {
        public int Number;
    }

    public class SimpleThreadModel {
        protected ConcurrentQueue<QueueData> queues;
        protected int threadCount;
        protected List<Task> tasks;

        public SimpleThreadModel() {
            threadCount = 2;
            InitTasks();
        }

        protected void InitTasks() {
            tasks = new List<Task>();
            for (int i = 0; i < threadCount; i++) {
                tasks.Add(new Task(ThreadHandler.Handle, queues));
            }
        }

        public void RunTasks() {
            foreach (var task in tasks) task.Start();

            foreach (var task in tasks) task.Wait();
        }
    }
}
