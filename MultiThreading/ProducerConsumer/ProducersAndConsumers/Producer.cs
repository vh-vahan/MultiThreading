using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.ProducerConsumer.ProducersAndConsumers
{
    public class Producer
    {
        IDelegator delegator;

        public Producer(IDelegator delegator)
        {
            this.delegator = delegator;
        }

        public void Produce<T>(T message) where T : BaseMessage
        {
            delegator.AddMessage(message);
        }

    }
}
