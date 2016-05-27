using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class MonitorWaitPulseUsage
    {
        static readonly object locker = new object();
        static bool go;
        public static void Start()
        { 
            // The new thread will block
            new Thread(Work).Start(); // because go==false.
            Console.ReadLine(); // Wait for user to hit Enter
            lock (locker) // Let's now wake up the thread by
            { 
                // setting go=true and pulsing.
                go = true;
                Monitor.Pulse(locker);
            }
        }
        static void Work()
        {
            lock (locker)
                while (!go)
                    Monitor.Wait(locker); // Lock is released while we’re waiting
            Console.WriteLine("Woken");
        }
    }


}
