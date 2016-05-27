using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class MonitorUsage
    {
        static readonly object locker = new object();
        static int val1, val2;
        static void Go()
        {
            lock (locker)
            {
                if (val2 != 0)
                    Console.WriteLine(val1 / val2);
                val2 = 0;
            }



            Monitor.Enter(locker);
            try
            {
                if (val2 != 0)
                    Console.WriteLine(val1 / val2);
                val2 = 0;
            }
            finally
            {
                Monitor.Exit(locker);
            }



            bool lockTaken = false;
            try
            {
                Monitor.Enter(locker, ref lockTaken);
                // Do some thing
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(locker);
            }

        }
    }
}
