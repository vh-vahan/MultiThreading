using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class SpinWaitUsage
    {
        bool proceed;
        public void Test()
        {
            SpinWait.SpinUntil(() =>
            {
                Thread.MemoryBarrier();
                return proceed;
            });

            var spinWait = new SpinWait();
            while (!proceed)
            {
                Thread.MemoryBarrier();
                spinWait.SpinOnce();
            }
        }

        
    }
}
