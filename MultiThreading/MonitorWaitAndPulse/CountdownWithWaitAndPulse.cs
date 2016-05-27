using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.WaitAndPulse
{
    public class CountdownWithWaitAndPulse
    {
        object locker = new object();
        int value;
        public CountdownWithWaitAndPulse() { }
        public CountdownWithWaitAndPulse(int initialCount)
        {
            value = initialCount;
        }
        public void Signal()
        {
            AddCount(-1);
        }
        public void AddCount(int amount)
        {
            lock (locker)
            {
                value += amount;
                if (value <= 0)
                    Monitor.PulseAll(locker);
            }
        }
        public void Wait()
        {
            lock (locker)
                while (value > 0)
                    Monitor.Wait(locker);
        }
    }
}
