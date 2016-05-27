using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class LockFreeUpdatesUsingSpinWait
    {
        public static void MultiplyXByLockFree(int x, int factor)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                int snapshot1 = x;
                Thread.MemoryBarrier();
                int calc = snapshot1 * factor;
                int snapshot2 = Interlocked.CompareExchange(ref x, calc, snapshot1);
                if (snapshot1 == snapshot2)
                    return;
                spinWait.SpinOnce();
            }
        }

        public static void LockFreeUpdate<T>(ref T field, Func<T, T> updateFunction) where T : class
        {
            var spinWait = new SpinWait();
            while (true)
            {
                T snapshot1 = field;
                T calc = updateFunction(snapshot1);
                T snapshot2 = Interlocked.CompareExchange(ref field, calc, snapshot1);
                if (snapshot1 == snapshot2)
                    return;
                spinWait.SpinOnce();
            }
        }


        EventHandler someDelegate;
        public event EventHandler SomeEvent
        {
            add { LockFreeUpdate(ref someDelegate, d => d + value); }
            remove { LockFreeUpdate(ref someDelegate, d => d - value); }
        }



        public class ProgressStatus // Immutable class
        {
            public readonly int PercentComplete;
            public readonly string Message;
            public ProgressStatus(int percentComplete, string message)
            {
                PercentComplete = percentComplete;
                Message = message;
            }
        }

        public void Update()
        {
            ProgressStatus status = new ProgressStatus(0, "Starting");
            LockFreeUpdate(ref status, s => new ProgressStatus(s.PercentComplete + 1, s.Message));
        }
    }
}
