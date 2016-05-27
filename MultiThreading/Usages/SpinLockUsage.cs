using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class SpinLockUsage
    {
        void Caller()
        {
            var spinLock = new SpinLock(true); // Enable owner tracking
            bool lockTaken = false;
            try
            {
                spinLock.Enter(ref lockTaken);
                // Do some thing
            }
            finally
            {
                if (lockTaken)
                    spinLock.Exit();
            }
        }

    }

}
