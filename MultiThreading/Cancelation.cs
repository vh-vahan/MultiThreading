using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{

    public class Canceler
    {
        object cancelLocker = new object();
        bool cancelRequest;
        public bool IsCancellationRequested
        {
            get
            {
                lock (cancelLocker)
                    return cancelRequest;
            }
        }
        public void Cancel()
        {
            lock (cancelLocker)
                cancelRequest = true;
        }
        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested)
                throw new OperationCanceledException();
        }
    }
    public class TestCanceler
    {
        /*
         * 
         * var cancelSource = new CancellationTokenSource();
         * new Thread (() => Work (cancelSource.Token)).Start();
         * void Work (CancellationToken cancelToken)
            {
            cancelToken.ThrowIfCancellationRequested();
            ...
            }
        */

        public static void Caller()
        {
            var canceler = new Canceler();
            new Thread(() =>
            {
                try
                {
                    Work(canceler);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Canceled");
                }
            }).Start();
            Thread.Sleep(1000);
            canceler.Cancel();
        }
        static void Work(Canceler c)
        {
            while (true)
            {
                c.ThrowIfCancellationRequested();
                // ...
                try
                {
                    OtherMethod(c);
                }
                finally { /* any required cleanup */ }
            }
        }
        static void OtherMethod(Canceler c)
        {
            // Do some thing...
            c.ThrowIfCancellationRequested();
        }
    }
}
