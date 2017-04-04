using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.ProducerConsumer.ProducersAndConsumers
{
    public class Client
    {

        public static void Run()
        {
            Delegator dg = new Delegator(5);

            Consumer<MessagePerson> personsConsumer = new Consumer<MessagePerson>();
            Consumer<MessageCompany> companiesConsumer = new Consumer<MessageCompany>();

            dg.Register(personsConsumer);
            dg.Register(companiesConsumer);

            Producer prod = new Producer(dg);
            prod.Produce(new MessageCompany() { Id = Guid.NewGuid(), Address = "Address1", CompanyName = "CompanyName1" });
            prod.Produce(new MessageCompany() { Id = Guid.NewGuid(), Address = "Address2", CompanyName = "CompanyName2" });
            prod.Produce(new MessagePerson() { Id = Guid.NewGuid(),  FirstName = "Vahan" , Surname = "Hovhannisyan" });

        }
    }
}
