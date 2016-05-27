using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class MemoryFenciesUsage
    {
        int answer;
        bool complete;
        public void A()
        {
            answer = 123;
            Thread.MemoryBarrier(); // Barrier 1
            complete = true;
            Thread.MemoryBarrier(); // Barrier 2
        }
        public void B()
        {
            Thread.MemoryBarrier(); // Barrier 3
            if (complete)
            {
                Thread.MemoryBarrier(); // Barrier 4
                Console.WriteLine(answer);
            }
        }


        int answer1, answer2, answer3;
        public void A1()
        {
            answer1 = 1; answer2 = 2; answer3 = 3;
            Thread.MemoryBarrier();
            complete = true;
            Thread.MemoryBarrier();
        }
        public void B1()
        {
            Thread.MemoryBarrier();
            if (complete)
            {
                Thread.MemoryBarrier();
                Console.WriteLine(answer1 + answer2 + answer3);
            }
        }
    }
}
