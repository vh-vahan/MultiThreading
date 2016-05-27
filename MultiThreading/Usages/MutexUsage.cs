using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class MutexUsage
    {
        public static void Start()
        {
            // Naming a Mutex makes it available computer-wide.
            using (var mutex = new Mutex(false, "Unique Name"))
            {
                // Wait a few seconds if contended, in case another instance
                // of the program is still in the process of shutting down.
                if (!mutex.WaitOne(TimeSpan.FromSeconds(3), false))
                {
                    Console.WriteLine("Another instance of the app is running. Bye!");
                    return;
                }
                RunProgram();
            }
        }
        static void RunProgram()
        {
            Console.WriteLine("Running. Press Enter to exit");
            Console.ReadLine();
        }
    }

}