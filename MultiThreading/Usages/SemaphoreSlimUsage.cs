using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class SemaphoreSlimUsage
    {
        static SemaphoreSlim sem = new SemaphoreSlim(3); // Capacity of 3
        public static void Start()
        {
            for (int i = 1; i <= 5; i++)
                new Thread(Enter).Start(i);
        }
        static void Enter(object id)
        {
            Console.WriteLine(id + " wants to enter");
            sem.Wait();
            Console.WriteLine(id + " is in!"); 
            Thread.Sleep(1000 * (int)id); 
            Console.WriteLine(id + " is leaving");
            sem.Release();
        }
    }

}
