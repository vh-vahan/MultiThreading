using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.ProducerConsumer.ProducersAndConsumers
{
    public interface IConsumer
    {
        void Consume(BaseMessage message);
        bool CanHandleMessage { get; }
    }

    public class Consumer<T> : IConsumer where T : BaseMessage
    {
        public Type TypeToHandle { get; } = typeof(T);

        private Action<T> processor;

        bool canHandleMessage = true;

        public Consumer() : this((msg) => { Console.WriteLine(msg.Id); })
        {

        }
        public Consumer(Action<T> processor)
        {
            this.processor = processor;
        }

        public void Process(T message)
        {
            canHandleMessage = false;
            processor(message);
            canHandleMessage = true;
        }

        public void Consume(BaseMessage message) => Process((T)message);

        public bool CanHandleMessage
        {
            get
            {
                return canHandleMessage;
            }
        }
    }
}
