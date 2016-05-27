using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Usages
{
    public class InterlockedUsage
    {
        static long sum;
        public static void Start()
        {
            Interlocked.Increment(ref sum);
            Interlocked.Decrement(ref sum);

            Interlocked.Add(ref sum, 3);

            Console.WriteLine(Interlocked.Read(ref sum));

            Console.WriteLine(Interlocked.Exchange(ref sum, 10));

            Console.WriteLine(Interlocked.CompareExchange(ref sum, 123, 10));
        }
    }
}
