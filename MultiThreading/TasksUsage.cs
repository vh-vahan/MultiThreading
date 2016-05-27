using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MultiThreading
{
    public class TasksUsage
    {
        public static void Start()
        {

            Task parent = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("I am a parent");
                Task.Factory.StartNew(() => // Detached task
                {
                    Console.WriteLine("I am detached");
                });
                Task.Factory.StartNew(() => // Child task
                {
                    Console.WriteLine("I am a child");
                }, TaskCreationOptions.AttachedToParent);
            });



            TaskCreationOptions atp = TaskCreationOptions.AttachedToParent;
            var parent1 = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() => // Child
                {
                    Task.Factory.StartNew(() => { throw null; }, atp); // Grandchild
                }, atp);
            });
            // The following call throws a NullReferenceException (wrapped in nested AggregateExceptions):
            parent1.Wait();



            var cancelSource = new CancellationTokenSource();
            CancellationToken token = cancelSource.Token;
            Task task = Task.Factory.StartNew(() =>
            {
                // Do some thing.
                token.ThrowIfCancellationRequested();
                // Do some thing.
            }, token);
            cancelSource.Cancel();
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is OperationCanceledException)
                    Console.Write("Task canceled!");
            }




            Task task1 = Task.Factory.StartNew(() => Console.Write("antecedant.."));
            Task task2 = task1.ContinueWith(ant => Console.Write("..continuation"));
            Task.Factory.StartNew<int>(() => 8)
                    .ContinueWith(ant => ant.Result * 2)
                    .ContinueWith(ant => Math.Sqrt(ant.Result))
                    .ContinueWith(ant => Console.WriteLine(ant.Result)); // 4


            Task continuation = Task.Factory.StartNew(() => { throw null; })
                        .ContinueWith(ant =>
                        {
                            if (ant.Exception != null)
                                throw ant.Exception;
                            // Continue processing...
                        });
            continuation.Wait(); // Exception is now thrown back to caller.



            Task task3 = Task.Factory.StartNew(() => { throw null; });
            Task error = task3.ContinueWith(ant => Console.Write(ant.Exception), TaskContinuationOptions.OnlyOnFaulted);
            Task ok = task3.ContinueWith(ant => Console.Write("Success"), TaskContinuationOptions.NotOnFaulted);


            var source = new TaskCompletionSource<int>();
            new Thread(() => { Thread.Sleep(5000); source.SetResult(123); }).Start();
            Task<int> t = source.Task;
            Console.WriteLine(t.Result);
        }

    }

    public partial class MyWindow : Window
    {
        TaskScheduler uiScheduler;
        Label lblResult = new Label();

        public MyWindow()
        {      
            // Get the UI scheduler for the thread that created the form:
            uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew<string>(SomeComplexWebService).ContinueWith(ant => lblResult.Content = ant.Result, uiScheduler);
        }
        string SomeComplexWebService()
        {
            return "done";
        }
    }

    public partial class MyWindow1 : Window
    {
        TextBox txtMessage = new TextBox();
        public MyWindow1()
        {
            new Thread(Work).Start();
        }
        void Work()
        {
            Thread.Sleep(5000); // Simulate time-consuming task
            UpdateMessage("The answer");
        }
        void UpdateMessage(string message)
        {
            Action action = () => txtMessage.Text = message;
            Dispatcher.Invoke(action);
            //this.Invoke(action);
        }
    }
}
