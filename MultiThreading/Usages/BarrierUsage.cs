using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    //Rendezvous
    public class BarrierUsage
    {
        //static Barrier _barrier = new Barrier(3, barrier => Console.WriteLine());
        static Barrier barrier = new Barrier(3);
        public static void Start()
        {
            new Thread(Run).Start();
            new Thread(Run).Start();
            new Thread(Run).Start();
        }
        static void Run()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.Write(i + " ");
                barrier.SignalAndWait();
            }
        }
    }
}
