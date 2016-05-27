using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.WaitAndPulse
{
    public class RendezvousWithWaitAndPulse
    {
        static object locker = new object();
        static CountdownWithWaitAndPulse countdown = new CountdownWithWaitAndPulse(2);
        public static void Start()
        {
            // Get each thread to sleep a random amount of time.
            Random r = new Random();
            new Thread(Run).Start(r.Next(10000));
            Thread.Sleep(r.Next(10000));

            countdown.Signal();
            countdown.Wait();

            Console.Write("we are here");
        }
        static void Run(object delay)
        {
            Thread.Sleep((int)delay);

            countdown.Signal();
            countdown.Wait();

            Console.Write("we are here");
        }
    }
}
