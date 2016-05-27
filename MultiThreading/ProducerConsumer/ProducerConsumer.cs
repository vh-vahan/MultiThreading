using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.ProducerConsumer
{
    public class ProducerConsumer<T> : IDisposable where T : class
    {
        CancellationTokenSource tokenSource;
        CancellationToken token;

        class ConsumerInfo
        {
            public Consumer<T> consumer;
            public Task associatedTaks;
            public CancellationTokenSource tokenSource;
            public CancellationToken token;
        }


        public ProducerConsumer()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
        }

        BlockingCollection<T> msgs = new BlockingCollection<T>();
        Dictionary<Consumer<T>, ConsumerInfo> consumers = new Dictionary<Consumer<T>, ConsumerInfo>();

        public Task AddConsumer(Consumer<T> consumer)
        {
            return AddConsumer(consumer, token);
        }

        public Task AddConsumer(Consumer<T> consumer, CancellationToken token)
        {
            ConsumerInfo info = new ConsumerInfo()
            {
                consumer = consumer,
                tokenSource = this.token == token ? tokenSource : CancellationTokenSource.CreateLinkedTokenSource(token),
                token = token
            };
            Task task = new Task(Consume, info, token);
            info.associatedTaks = task;
            task.Start();
            consumers.Add(consumer, info);
            return task;
        }

        public void RemoveConsumer(Consumer<T> consumer)
        {
            ConsumerInfo info = consumers[consumer];
            info.tokenSource.Cancel();
            try
            {
                info.associatedTaks.Wait();
            }
            catch (Exception)
            {
                //log
                //throw;
            }

            consumers.Remove(consumer);
        }
        public async Task RemoveConsumerAsync(Consumer<T> consumer)
        {
            ConsumerInfo info = consumers[consumer];
            info.tokenSource.Cancel();
            await info.associatedTaks.ContinueWith(t => consumers.Remove(consumer));
            //await info.associatedTaks;
            //consumers.Remove(consumer);
        }

        public void AddMsg(T msg)
        {
            msgs.Add(msg);
        }

        void Consume(object consumerInfo)
        {
            ConsumerInfo info = consumerInfo as ConsumerInfo;
            foreach (var item in msgs.GetConsumingEnumerable())
            {
                info.token.ThrowIfCancellationRequested();
                try
                {
                    info.consumer.Process(item);
                }
                catch (Exception)
                {
                    //log
                    //throw;
                }

            }
        }

        public void Dispose()
        {
            msgs.CompleteAdding();
        }
    }



    public abstract class Consumer<T> where T : class
    {
        public abstract void Process(T msg);
    }

    public class ConsumerA<T> : Consumer<T> where T : class
    {
        public override void Process(T msg)
        {
            Console.WriteLine(msg.ToString());
        }
    }



    public class ProducerConsumerClient
    {

        void Start()
        {
            ProducerConsumer<string> pc = new ProducerConsumer<string>();

            Consumer<string> consumer1 = new ConsumerA<string>();
            Task task1 = pc.AddConsumer(consumer1);


            Consumer<string> consumer2 = new ConsumerA<string>();
            CancellationTokenSource cts = new CancellationTokenSource();
            Task task2 = pc.AddConsumer(consumer1, cts.Token);



            pc.AddMsg("msg1");
            pc.AddMsg("msg2");
            pc.AddMsg("msg3");
            pc.AddMsg("msg4");

        }

    }


}
