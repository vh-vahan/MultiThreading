using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class ReaderWriterLockSlimUsage
    {
        static ReaderWriterLockSlim rw = new ReaderWriterLockSlim();
        static List<int> items = new List<int>();
        static Random rand = new Random();
        public static void Start()
        {
            new Thread(Read).Start();
            new Thread(Read).Start();
            new Thread(Read).Start();
            new Thread(Write).Start("11");
            new Thread(Write).Start("22");
        }

        public static void Read()
        {
            while (true)
            {
                rw.EnterReadLock();
                foreach (int i in items)
                    Thread.Sleep(10);
                rw.ExitReadLock();
            }
        }
        public static void Write(object threadID)
        {
            while (true)
            {
                int newNumber = GetRandNum(100);
                rw.EnterWriteLock();
                items.Add(newNumber);
                rw.ExitWriteLock();
                Console.WriteLine("Thread " + threadID + " added " + newNumber);
                Thread.Sleep(100);
            }
        }
        public static void WriteUpgradeable(object threadID)
        {
            while (true)
            {
                int newNumber = GetRandNum(100);
                rw.EnterUpgradeableReadLock();
                if (!items.Contains(newNumber))
                {
                    rw.EnterWriteLock();
                    items.Add(newNumber);
                    rw.ExitWriteLock();
                    Console.WriteLine("Thread " + threadID + " added " + newNumber);
                }
                rw.ExitUpgradeableReadLock();
                Thread.Sleep(100);
            }
        }


        static int GetRandNum(int max)
        {
            lock (rand)
                return rand.Next(max);
        }
    }

}
