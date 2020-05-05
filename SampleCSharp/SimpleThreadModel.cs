using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;


namespace SampleCSharp
{
    public class ThreadHandler {
        public static void Handle(object state) {
        }
    }

    public class QueueData {

    }

    public class SimpleThreadModel {
        protected ConcurrentQueue<QueueData> queues;
        protected int threadCount;
        protected List<Task> tasks;

        public SimpleThreadModel() {
            threadCount = 4;
            InitTasks();
        }

        protected void InitTasks() {
            tasks = new List<Task>();
            for (int i = 0; i < threadCount; i++) {
                tasks.Add(new Task(ThreadHandler.Handle, queues));
            }
        }

        protected void StartTasks() {
            foreach (var task in tasks) task.Start();
        }
    }
}
