using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class Client
    {
        public FinancialWorker GetFinancialTotalsBackground(int foo, int bar)
        {
            return new FinancialWorker(foo, bar);
        }
    }
    public class FinancialWorker : BackgroundWorker
    {
        public Dictionary<string, int> Result;
        public readonly int Foo, Bar;
        public FinancialWorker()
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }
        public FinancialWorker(int foo, int bar) : this()
        {
            this.Foo = foo; this.Bar = bar;
        }
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            bool flag = false;
            ReportProgress(0, "Working hard on this report...");

            while (!flag) 
            {
                if (CancellationPending) { e.Cancel = true; return; }
                // Perform another calculation step ...
                // ...
                ReportProgress(40, "Getting there...");

                flag = true;
            }
            ReportProgress(100, "Done!");
            e.Result = Result = new Dictionary<string, int>();
        }
    }

}
