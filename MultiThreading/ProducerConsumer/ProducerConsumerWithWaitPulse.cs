
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class ProducerConsumerWithWaitPulse
    {
        readonly object locker = new object();
        Thread[] consumerThreads;
        Queue<Action> tasks = new Queue<Action>();
        public ProducerConsumerWithWaitPulse(int consumerCount)
        {
            consumerThreads = new Thread[consumerCount];
            for (int i = 0; i < consumerCount; i++)
                (consumerThreads[i] = new Thread(Consume)).Start();
        }
        public void Close(bool waitForConsumers)
        {
            foreach (Thread worker in consumerThreads)
                EnqueueItem(null);
            if (waitForConsumers)
                foreach (Thread consumer in consumerThreads)
                    consumer.Join();
        }
        public void EnqueueItem(Action item)
        {
            lock (locker)
            {
                tasks.Enqueue(item); 
                Monitor.Pulse(locker);
            }
        }
        void Consume()
        {
            while (true) 
            {
                Action task;
                lock (locker)
                {
                    while (tasks.Count == 0)
                        Monitor.Wait(locker);
                    task = tasks.Dequeue();
                }
                if (task == null)
                    return; 
                task(); 
            }
        }




        public static void Usage()
        {
            ProducerConsumerWithWaitPulse q = new ProducerConsumerWithWaitPulse(2);
            Console.WriteLine("Enqueuing 10 items...");
            for (int i = 0; i < 10; i++)
            {
                int itemNumber = i;
                q.EnqueueItem(() =>
                {
                    Thread.Sleep(1000); 
                    Console.Write(" Task" + itemNumber);
                });
            }
            q.Close(true);
            Console.WriteLine("done");
        }

    }


}
