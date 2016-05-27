using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class CountdownEventUsage
    {
        static CountdownEvent countdown = new CountdownEvent(3);
        public static void Start()
        {
            new Thread(Work).Start("I am thread 1");
            new Thread(Work).Start("I am thread 2");
            new Thread(Work).Start("I am thread 3");
            countdown.Wait(); // Blocks until Signal has been called 3 times
            Console.WriteLine("All threads have finished work");
        }
        static void Work(object thing)
        {
            Thread.Sleep(1000);
            Console.WriteLine(thing);
            countdown.Signal();
        }
    }
}
