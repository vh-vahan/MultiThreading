using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.WaitAndPulse
{
    public class MonitorWaitPulseUsageForSyncWork
    {
        static readonly object locker = new object();
        static bool ready, go;
        static void Start()
        {
            new Thread(Run).Start();
            for (int i = 0; i < 5; i++)
                lock (locker)
                {
                    while (!ready)
                        Monitor.Wait(locker);
                    ready = false;
                    go = true;
                    Monitor.PulseAll(locker);
                }
        }
        static void Run()
        {
            for (int i = 0; i < 5; i++)
                lock (locker)
                {
                    ready = true;
                    Monitor.PulseAll(locker);
                    while (!go)
                        Monitor.Wait(locker);
                    go = false;
                    Console.WriteLine("done");
                }
        }
    }
}
