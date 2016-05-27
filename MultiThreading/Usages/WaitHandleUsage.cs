using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class WaitHandleUsage
    {
        //var auto = new EventWaitHandle(false, EventResetMode.AutoReset);
        static EventWaitHandle waitHandle = new AutoResetEvent(false);
        public static void Start()
        {
            new Thread(Waiter).Start();
            Thread.Sleep(1000); // Pause for a second...
            waitHandle.Set(); // Wake up the Waiter.
        }
        static void Waiter()
        {
            Console.WriteLine("Waiting...");
            waitHandle.WaitOne(); // Wait for notification
            Console.WriteLine("Notified");
        }
    }

}
