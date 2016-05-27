using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class TwoWaySignaling
    {
        static EventWaitHandle ready = new AutoResetEvent(false);
        static EventWaitHandle go = new AutoResetEvent(false);
        static readonly object locker = new object();
        static string message;
        public static void Start()
        {
            new Thread(Work).Start();
            ready.WaitOne(); // First wait until worker is ready
            lock (locker)
                message = "aaa";
            go.Set(); // Tell worker to go

            ready.WaitOne();
            lock (locker)
                message = "bbb"; // Give the worker another message
            go.Set();

            ready.WaitOne();
            lock (locker)
                message = null; // Signal the worker to exit
            go.Set();
        }
        static void Work()
        {
            while (true)
            {
                ready.Set(); // Indicate that we're ready
                go.WaitOne(); // Wait to the signal to start.
                lock (locker)
                {
                    if (message == null)
                        return; //exit
                    Console.WriteLine(message);
                }
            }
        }
    }
}
