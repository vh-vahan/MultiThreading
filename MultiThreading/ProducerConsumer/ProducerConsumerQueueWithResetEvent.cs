using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class ProducerConsumerQueueWithResetEvent : IDisposable
    {
        EventWaitHandle wh = new AutoResetEvent(false);
        Thread consumerThread;
        readonly object locker = new object();
        Queue<string> tasks = new Queue<string>();
        public ProducerConsumerQueueWithResetEvent()
        {
            consumerThread = new Thread(Consume);
            consumerThread.Start();
        }
        public void EnqueueTask(string task)
        {
            lock (locker)
                tasks.Enqueue(task);
            wh.Set();
        }
        public void Dispose()
        {
            EnqueueTask(null); // Signal the consumer to exit.
            consumerThread.Join(); // Wait for the consumer's thread to finish.
            wh.Close(); // Release any OS resources.
        }
        void Consume()
        {
            while (true)
            {
                string task = null;
                lock (locker)
                    if (tasks.Count > 0)
                    {
                        task = tasks.Dequeue();
                        if (task == null)
                            return;
                    }
                if (task != null)
                {
                    Console.WriteLine("Performing task: " + task);
                    Thread.Sleep(1000);
                }
                else
                    wh.WaitOne(); // No more tasks - wait for a signal
            }
        }





        public static void Usage()
        {
            using (ProducerConsumerQueueWithResetEvent q = new ProducerConsumerQueueWithResetEvent())
            {
                q.EnqueueTask("Starting task");
                for (int i = 0; i < 10; i++)
                    q.EnqueueTask("Task " + i);
                q.EnqueueTask("Last task");
            }
        }
    }





}
