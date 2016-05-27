using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class ThreadPoolUsage
    {
        static ManualResetEvent _starter = new ManualResetEvent(false);
        public static void Start()
        {
            RegisteredWaitHandle reg = ThreadPool.RegisterWaitForSingleObject(_starter, Go, "Some Data", -1, true);
            Thread.Sleep(5000);
            Console.WriteLine("Signaling worker...");
            _starter.Set();
            Console.ReadLine();
            reg.Unregister(_starter); // Clean up when we are done.


            ThreadPool.SetMinThreads(50, 50);

            Func<string, int> method = Work;
            IAsyncResult cookie = method.BeginInvoke("test", null, null);
            //
            // ... here we can do other work in parallel...
            //
            int result = method.EndInvoke(cookie);
            Console.WriteLine("String length is: " + result);

            method.BeginInvoke("test", Done, method);


            //bool blocked = (t.ThreadState & ThreadState.WaitSleepJoin) != 0;

            ThreadPool.QueueUserWorkItem(Go);
            ThreadPool.QueueUserWorkItem(Go, 123);
            Console.ReadLine();
        }

        static void Done(IAsyncResult cookie)
        {
            var target = (Func<string, int>)cookie.AsyncState;
            int result = target.EndInvoke(cookie);
            Console.WriteLine("String length is: " + result);
        }
        static int Work(string s)
        {
            return s.Length;
        }
        static void Go(object data, bool timedOut)
        {
            Console.WriteLine("Started - " + data);
            // Perform task...
        }
        static void Go(object state)
        {
            // Declare and use a local variable - 'cycles'
            for (int cycles = 0; cycles < 5; cycles++)
                Console.Write('?');
        }
    }
}
