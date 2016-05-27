using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.WaitAndPulse
{
    public class WaitHandleWithWaitAndPulse
    {

        readonly object locker = new object();
        bool flag1, flag2, flag3;
        public void WaitAll()
        {
            lock (locker)
                while (!flag1 && !flag2 && !flag3)
                    Monitor.Wait(locker);
        }
        public void WaitAny()
        {
            lock (locker)
                while (!flag1 || !flag2 || !flag3)
                    Monitor.Wait(locker);
        }
    }
}
