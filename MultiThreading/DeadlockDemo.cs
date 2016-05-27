using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class DeadlockDemo
    {
        public static void Start()
        {
            object locker1 = new object();
            object locker2 = new object();
            new Thread(() =>
            {
                lock (locker1)
                {
                    Thread.Sleep(1000);
                    lock (locker2) ; // Deadlock
                }
            }).Start();
            lock (locker2)
            {
                Thread.Sleep(1000);
                lock (locker1) ; // Deadlock
            }
        }
    }
}
