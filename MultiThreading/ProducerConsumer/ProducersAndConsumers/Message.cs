using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.ProducerConsumer.ProducersAndConsumers
{

    public interface ICancelable
    {
        void Cancel();
    }

    public abstract class BaseMessage : ICancelable
    {
        protected BaseMessage(CancellationTokenSource cts)
        {
            this.cts = cts;
        }

        protected CancellationTokenSource cts { get; set; }
        public Guid Id { get; set; }

        public void Cancel()
        {
            cts.Cancel();
        }
    }

    public class MessagePerson : BaseMessage
    {
        public MessagePerson() : this(new CancellationTokenSource())
        {

        }
        public MessagePerson(CancellationTokenSource cts) : base(cts)
        {

        }
        public string FirstName { get; set; }
        public string Surname { get; set; }
    }

    public class MessageCompany : BaseMessage
    {
        public MessageCompany() : this(new CancellationTokenSource())
        {

        }
        public MessageCompany(CancellationTokenSource cts) : base(cts)
        {

        }
        public string CompanyName { get; set; }
        public string Address { get; set; }
    }


}
