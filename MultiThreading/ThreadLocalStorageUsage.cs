using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class ThreadLocalStorageUsage
    {
        [ThreadStatic]
        static int x;
        static ThreadLocal<int> y = new ThreadLocal<int>(() => 3);
        ThreadLocal<Random> localRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));


        LocalDataStoreSlot secSlot = Thread.GetNamedDataSlot("securityLevel");
        // This property has a separate value on each thread.
        public int SecurityLevel
        {
            get
            {
                object data = Thread.GetData(secSlot);
                return data == null ? 0 : (int)data;
            }
            set
            {
                Thread.SetData(secSlot, value);
            }
        }

    }



}
