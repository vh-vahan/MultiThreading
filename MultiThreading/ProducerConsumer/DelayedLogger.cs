using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class DelayedLogger
    {
        static Logger loger = new Logger() { Delay = TimeSpan.FromSeconds(20) };
        //static AutoResetEvent ev = new AutoResetEvent(false);
        static void Run()
        {

            //ThreadPool.QueueUserWorkItem(o =>
            //   {
            //       var t = loger.Log("message 1");
            //       Thread.Sleep(TimeSpan.FromSeconds(10));
            //       t.ts.SetResult(t.Id);
            //   }
            //);

            //ThreadPool.QueueUserWorkItem(o =>
            //   {
            //       var t = loger.Log("message 2");
            //       Thread.Sleep(TimeSpan.FromSeconds(40));
            //       t.ts.SetResult(t.Id);
            //   }
            //);



            for (int i = 0; i < 2000; i++)
            {
                int tmp = i;
                ThreadPool.QueueUserWorkItem(o =>
                    {
                        Random rnd = new Random();
                        TimeSpan duration = TimeSpan.FromSeconds(rnd.Next(5, 120));
                        Guid Id;
                        var t = loger.Log($"message {tmp}", duration, out Id);
                        Thread.Sleep(duration);
                        t.SetResult(Id);
                    }
                );
            }

            /*
            ThreadPool.QueueUserWorkItem(o =>
            {
                var res = ThreadPool.RegisterWaitForSingleObject(ev, loger.Log, "message 3", TimeSpan.FromMinutes(5).Milliseconds, false);
                Thread.Sleep(TimeSpan.FromMinutes(1));
                ev.Set();
            }
           );
            ThreadPool.QueueUserWorkItem(o =>
            {
                var res = ThreadPool.RegisterWaitForSingleObject(ev, loger.Log, "message 4", TimeSpan.FromMinutes(5).Milliseconds, false);
                Thread.Sleep(TimeSpan.FromMinutes(7));
                ev.Set();
            }
            );
            */

            Thread.Sleep(Timeout.Infinite);
        }
    }



    class LogggingStore : IDisposable
    {
        Thread consumer;
        ConcurrentDictionary<Guid, Task<Guid>> tasks = new ConcurrentDictionary<Guid, Task<Guid>>();
        ConcurrentDictionary<Guid, LogInfo> store = new ConcurrentDictionary<Guid, LogInfo>();

        static Lazy<LogggingStore> instance = new Lazy<LogggingStore>(() => new LogggingStore(), true);
        public static LogggingStore Instance { get { return instance.Value; } }

        TaskCompletionSource<Guid> DefaultCompletionSource = new TaskCompletionSource<Guid>();
        private LogggingStore()
        {
            tasks.TryAdd(Guid.Empty, DefaultCompletionSource.Task);

            consumer = new Thread(Consume);
            consumer.Start();
        }

        public void Enqueue(LogInfo info, TaskCompletionSource<Guid> ts)
        {
            if (store.TryAdd(info.Id, info))
                tasks.TryAdd(info.Id, ts.Task);
        }

        private void Consume()
        {
            while (true)
            {
                int index = Task.WaitAny(tasks.Values.ToArray(), TimeSpan.FromSeconds(20));
                if(tasks.Count == 1)
                {
                    Console.WriteLine("DONE!!!!!");
                }
                if (index == -1)
                {
                    store.Values.Where(i => i.IsLate).ToList().ForEach(t =>
                    {
                        if (store.ContainsKey(t.Id))
                        {
                            LogInfo tmp;
                            Task<Guid> tmp1;
                            if (store.TryRemove(t.Id, out tmp))
                            {
                                if (tasks.TryRemove(t.Id, out tmp1))
                                {
                                    Console.WriteLine($"Started {tmp.start.ToString()} , Finished {DateTime.Now.ToString()} , TimedOut , Message {tmp.Message} AssumedDuratio {tmp.AssumedDuration} ");
                                }
                                else
                                {
                                    Console.WriteLine($"Unable to remove the task for {tmp.Id}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Unable to remove the info for {tmp.Id}");
                            }
                        }
                    });
                }
                else
                {
                    tasks.Values.Where(t => t.IsCompleted).ToList().ForEach(t =>
                    {
                        if (store.ContainsKey(t.Result))
                        {
                            var info = store[t.Result];
                            if (!info.IsLate)
                                Console.WriteLine($"Started {info.start.ToString()} , Finished {DateTime.Now.ToString()} , NormalCompletion , Message {info.Message} AssumedDuratio {info.AssumedDuration} ");

                            LogInfo tmp;
                            Task<Guid> tmp1;
                            if (store.TryRemove(t.Result, out tmp))
                            {
                                if(tasks.TryRemove(t.Result, out tmp1))
                                {
                                    
                                }
                                else
                                {
                                    Console.WriteLine($"Unable to remove the task for {tmp.Id}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Unable to remove the info for {tmp.Id}");
                            }
                        }
                    });
                }

                if(tasks.Count == 0)
                {
                    return;
                }
            }
        }

        public void Dispose()
        {
            DefaultCompletionSource.SetResult(Guid.Empty);
        }
    }

    //producer
    class Logger
    {
        public TimeSpan Delay { get; set; }

        public TaskCompletionSource<Guid> Log(object message, TimeSpan AssumedDuration, out Guid Id)
        {
            LogInfo info = new LogInfo()
            {

                Id = Guid.NewGuid(),
                Message = message,
                start = DateTime.Now,
                Delay = this.Delay,
                AssumedDuration = AssumedDuration,
            };

            var ts = new TaskCompletionSource<Guid>();

            LogggingStore.Instance.Enqueue(info, ts);
            Id = info.Id;
            return ts;
        }

        public void Log(object message, bool timedout)
        {
            if (!timedout)
                Console.WriteLine(message);
        }
    }


    class LogInfo
    {
        public object Message { get; set; }
        public Guid Id { get; set; }
        public DateTime start { get; set; }
        public TimeSpan Delay { get; set; }

        //for debugging 
        public TimeSpan AssumedDuration { get; set; }

        public bool IsLate { get { return DateTime.Now.Subtract(start) > Delay; } }
    }


}
