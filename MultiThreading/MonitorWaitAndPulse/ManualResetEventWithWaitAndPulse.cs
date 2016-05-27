using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.WaitAndPulse
{
    public class ManualResetEventWithWaitAndPulse
    {
        readonly object locker = new object();
        bool signal;

        public void WaitOne()
        {
            lock (locker)
            {
                while (!signal)
                    Monitor.Wait(locker);
            }
        }
        public void Set()
        {
            lock (locker)
            {
                signal = true;
                Monitor.PulseAll(locker);
            }
        }
        public void Reset()
        {
            lock (locker)
                signal = false;
        }
    }
}
