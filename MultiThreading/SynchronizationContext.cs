using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    [Synchronization]
    public class AutoLock : ContextBoundObject
    {
        public void Demo()
        {
            Console.Write("Start...");
            Thread.Sleep(1000);
            Console.WriteLine("end");
        }
    }
    public class TestAutoLock
    {
        public static void Start()
        {
            AutoLock safeInstance = new AutoLock();
            new Thread(safeInstance.Demo).Start();
            new Thread(safeInstance.Demo).Start();
            safeInstance.Demo();
        }
    }




    //[Synchronization(true)]
    [Synchronization]
    public class Deadlock : ContextBoundObject
    {
        public Deadlock Other;
        public void Demo()
        {
            Thread.Sleep(1000);
            Other.Hello();
        }
        public void Hello()
        {
            Console.WriteLine("hello");
        }
    }
    public class TestDeadlock
    {
        public static void Start()
        {
            Deadlock dead1 = new Deadlock();
            Deadlock dead2 = new Deadlock();
            dead1.Other = dead2;
            dead2.Other = dead1;
            new Thread(dead1.Demo).Start();
            dead2.Demo();
        }
    }


}
