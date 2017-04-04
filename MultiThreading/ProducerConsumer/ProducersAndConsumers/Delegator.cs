using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.ProducerConsumer.ProducersAndConsumers
{

    public interface IDelegator
    {
        void AddMessage(BaseMessage message);
        //bool Register<MessageType>(Consumer<MessageType> consumer) where MessageType : BaseMessage;
        //bool UnRegister<MessageType>() where MessageType : BaseMessage;
    }

    public class Delegator : IDelegator, IDisposable
    {
        ConcurrentDictionary<Type, IConsumer> map = new ConcurrentDictionary<Type, IConsumer>();
        BlockingCollection<BaseMessage> store = new BlockingCollection<BaseMessage>();
        List<Thread> workers = new List<Thread>();

        int consumingDegree;
        public Delegator() : this(1)
        {

        }
        public Delegator(int consumingDegree)
        {
            this.consumingDegree = consumingDegree;
            for (int i = 0; i < consumingDegree; i++)
            {
                Thread t = new Thread(DispatchMessages);
                workers.Add(t);
                t.Start();
            }
        }


        public void AddMessage(BaseMessage message)
        {
            store.Add(message);
        }
        void AddMessage(object message)
        {
            store.Add((BaseMessage)message);
        }
        public Task AddMessageAsync(BaseMessage message)
        {
            return Task.Factory.StartNew(AddMessage, message);
        }


        private void DispatchMessage(BaseMessage message)
        {
            var consumer = map[message.GetType()];
            SpinWait sw = new SpinWait();
            while (!consumer.CanHandleMessage)
            {
                sw.SpinOnce();
            }
            consumer.Consume(message);
        }
        private void DispatchMessages()
        {
            foreach (var message in store.GetConsumingEnumerable())
            {
                DispatchMessage(message);
            }
        }


        public bool Register<MessageType>(Consumer<MessageType> consumer) where MessageType : BaseMessage
        {
            return map.TryAdd(typeof(MessageType), consumer);
        }
        public bool UnRegister<MessageType>() where MessageType : BaseMessage
        {
            IConsumer removedConsumer;
            return map.TryRemove(typeof(MessageType), out removedConsumer);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
