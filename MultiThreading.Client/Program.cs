using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.Client
{
    class Program
    {
        static void Main(string[] args)
        {

            ProducerConsumer.ProducersAndConsumers.Client.Run();
        }
    }
}
