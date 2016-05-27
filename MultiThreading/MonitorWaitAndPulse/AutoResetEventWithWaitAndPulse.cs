using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.WaitAndPulse
{
    public class AutoResetEventWithWaitAndPulse
    {
        readonly object locker = new object();
        bool signal;
        public void WaitOne()
        {
            lock (locker)
            {
                while (!signal)
                {
                    Monitor.Wait(locker);
                }
                signal = false;
            }
        }
        public void Set()
        {
            lock (locker)
            {
                signal = true;
                Monitor.Pulse(locker);
            }
        }
        public void Reset()
        {
            lock (locker)
                signal = false;
        }
    }
}
