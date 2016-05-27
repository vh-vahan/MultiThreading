using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class AbortingThreads
    {
        public static void Interrupt()
        {
            Thread t = new Thread(delegate ()
            {
                try
                {
                    Thread.Sleep(Timeout.Infinite);
                }
                catch (ThreadInterruptedException)
                {
                    Console.Write("Forcibly Interrupted");
                }
                Console.WriteLine("Woken");
            });
            t.Start();
            t.Interrupt();
        }
        public static void AbortOrSuspend()
        {
            Thread t = new Thread(Work);
            t.Start();
            Thread.Sleep(1000); t.Abort();
            Thread.Sleep(1000); t.Abort();
            Thread.Sleep(1000); t.Abort();

            //------------------------------------//

            Thread suspendedThread = new Thread(Work);
            suspendedThread.Start();
            suspendedThread.Suspend();
            try
            {
                suspendedThread.Abort();
            }
            catch (ThreadStateException)
            {
                suspendedThread.Resume();
            }

        }
        public static void Work()
        {
            while (true)
            {
                try
                {
                    while (true) ;
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
                Console.WriteLine("I will not die");
            }
        }
    }
    public class AbortingThreads1
    {
        public static void Start()
        {
            while (true)
            {
                Thread t = new Thread(Work);
                t.Start();
                Thread.Sleep(100);
                t.Abort();
                Console.WriteLine("Aborted");
            }
        }
        public static void Work()
        {
            while (true)
                using (StreamWriter w = File.CreateText("myfile.txt")) { }
        }
    }
    public class AbortingThreads2
    {
        public static void Start()
        {
            while (true)
            {
                AppDomain ad = AppDomain.CreateDomain("worker");
                Thread t = new Thread(delegate () { ad.DoCallBack(Work); });
                t.Start();
                Thread.Sleep(100);
                t.Abort();
                if (!t.Join(2000))
                {
                    // Thread won't end - here's where we could take further action,
                    // if, indeed, there was anything we could do. Fortunately in
                    // this case, we can expect the thread *always* to end.
                }
                AppDomain.Unload(ad);
                Console.WriteLine("Aborted");
            }
        }
        public static void Work()
        {
            while (true)
                using (StreamWriter w = File.CreateText("myfile.txt")) { }
        }
    }

}
