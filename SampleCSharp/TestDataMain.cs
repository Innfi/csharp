using System;
using System.Threading;
using System.Threading.Tasks;


namespace SampleCSharp
{
    public class TestDataMain
    {
        public static void Tutorials() {
            Action<object> action = (object obj) => {
                Console.WriteLine($"Task={Task.CurrentId}. obj={obj}. " +
                    $"ThreadId={Thread.CurrentThread.ManagedThreadId}");
            };

            var t1 = new Task(action, "alpha");
            var t2 = Task.Factory.StartNew(action, "beta");
            t2.Wait();

            t1.Start();
            Console.WriteLine($"t1 launched. " +
                $"main thread={Thread.CurrentThread.ManagedThreadId}");
            t1.Wait();

            var taskData = "delta";
            var t3 = Task.Run(() => {
                Console.WriteLine($"Task:{Task.CurrentId}. obj={taskData}. " +
                    $"Thread={Thread.CurrentThread.ManagedThreadId}");
            });
            t3.Wait();

            var t4 = new Task(action, "gamma");
            t4.RunSynchronously();
            t4.Wait();
        }

        public static void RunSimpleThreadModel() {
            var model = new SimpleThreadModel();

            model.RunTasks();
        }

        public static void Main(string[] args)
        {
            RunSimpleThreadModel();
        }
    }
}
