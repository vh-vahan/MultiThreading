using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{

    public class Expensive
    {
    }

    public class ExpensiveUser
    {
        Expensive expensive;
        public Expensive Expensive // Lazily instantiate Expensive
        {
            get
            {
                if (expensive == null)
                    expensive = new Expensive();
                return expensive;
            }
        }

    }

    public class ExpensiveUserThreadSafe
    {
        Expensive expensive;
        readonly object expenseLock = new object();
        public Expensive Expensive
        {
            get
            {
                lock (expenseLock)
                {
                    if (expensive == null)
                        expensive = new Expensive();
                    return expensive;
                }
            }
        }

        public Expensive ExpensiveDoubleCheck
        {
            get
            {
                if (expensive == null)
                {
                    var instance = new Expensive();
                    lock (expenseLock)
                        if (expensive == null)
                            expensive = instance;
                }
                return expensive;
            }
        }

        volatile Expensive expensiveVolatile;
        public Expensive ExpensiveRaceForInit
        {
            get
            {
                if (expensiveVolatile == null)
                {
                    var instance = new Expensive();
                    Interlocked.CompareExchange(ref expensiveVolatile, instance, null);
                }
                return expensiveVolatile;
            }
        }

    }

    public class LazyInit
    {
        Lazy<Expensive> expensiveHolder = new Lazy<Expensive>(() => new Expensive(), true);
        public Expensive Expensive
        {
            get
            {
                return expensiveHolder.Value;
            }
        }


        Expensive expensive;
        public Expensive ExpensiveLazyInitializer
        {
            get // Implement double-checked locking
            {
                LazyInitializer.EnsureInitialized(ref expensive, () => new Expensive());
                return expensive;
            }
        }



    }
}
