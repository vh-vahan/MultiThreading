using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.WaitAndPulse
{
    public class SemaphoreWithWaitAndPulse
    {
        readonly object locker = new object();
        int allowed = 3;
        int signal;

        public SemaphoreWithWaitAndPulse(int capacity)
        {
            allowed = capacity;
        }
        public void WaitOne()
        {
            lock (locker)
            {
                signal++;
                while (signal > allowed)
                {
                    Monitor.Wait(locker);
                }

            }
        }
        public void Release()
        {
            lock (locker)
            {
                signal--;
                Monitor.Pulse(locker);
            }
        }
    }
}
