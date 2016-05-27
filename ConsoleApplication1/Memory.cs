using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    /* CLOSURE
    internal sealed class AClass
    {
        public static void UsingLocalVariablesInTheCallbackCode(Int32 numToDo)
        {
            // Some local variables
            Int32[] squares = new Int32[numToDo];
            AutoResetEvent done = new AutoResetEvent(false);
            // Do a bunch of tasks on other threads
            for (Int32 n = 0; n < squares.Length; n++)
            {
                ThreadPool.QueueUserWorkItem(
                obj => {
                Int32 num = (Int32)obj;
                    // This task would normally be more time consuming
                    squares[num] = num * num;
                    // If last task, let main thread continue running
                    if (Interlocked.Decrement(ref numToDo) == 0)
                        done.Set();
                }, n
                );
            }
            // Wait for all the other threads to finish
            done.WaitOne();
            // Show the results
            for (Int32 n = 0; n < squares.Length; n++)
                Console.WriteLine("Index {0}, Square={1}", n, squares[n]);
        }
    }
    internal sealed class AClass1
    {
        public static void UsingLocalVariablesInTheCallbackCode(Int32 numToDo)
        {
            // Some local variables
            WaitCallback callback1 = null;
            // Construct an instance of the helper class
            <> c__DisplayClass2 class1 = new <> c__DisplayClass2();
            // Initialize the helper class's fields
            class1.numToDo = numToDo;
            class1.squares = new Int32[class1.numToDo];
            class1.done = new AutoResetEvent(false);
            // Do a bunch of tasks on other threads
            for (Int32 n = 0; n < class1.squares.Length; n++)
            {
                if (callback1 == null)
                {
                    // New up delegate object bound to the helper object and
                    // its anonymous instance method
                    callback1 = new WaitCallback(
                    class1.< UsingLocalVariablesInTheCallbackCode > b__0);
                }
                ThreadPool.QueueUserWorkItem(callback1, n);
            }
            // Wait for all the other threads to finish
            class1.done.WaitOne();
            // Show the results
            for (Int32 n = 0; n < class1.squares.Length; n++)
                Console.WriteLine("Index {0}, Square={1}", n, class1.squares[n]);
        }


        // The helper class is given a strange name to avoid potential
        // conflicts and is private to forbid access from outside AClass
        [CompilerGenerated]
        private sealed class <>c__DisplayClass2 : Object {
        // One public field per local variable used in the callback code
        public Int32[] squares;
        public Int32 numToDo;
        public AutoResetEvent done;
        // public parameterless constructor
        public <>c__DisplayClass2 { }
        // Public instance method containing the callback code
        public void <UsingLocalVariablesInTheCallbackCode>b__0(Object obj)
        {
            Int32 num = (Int32)obj;
            squares[num] = num * num;
            if (Interlocked.Decrement(ref numToDo) == 0)
                done.Set();
        }

    }

*/


    public class WeakDelegate<TDelegate> where TDelegate : class
    {
        class MethodTarget
        {
            public readonly WeakReference Reference;
            public readonly MethodInfo Method;
            public MethodTarget(Delegate d)
            {
                Reference = new WeakReference(d.Target);
                Method = d.Method;
            }
        }

        List<MethodTarget> targets = new List<MethodTarget>();
        public WeakDelegate()
        {
            if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
                throw new InvalidOperationException("TDelegate must be a delegate type");
        }
        public void Combine(TDelegate target)
        {
            if (target == null)
                return;
            foreach (Delegate d in (target as Delegate).GetInvocationList())
                targets.Add(new MethodTarget(d));
        }
        public void Remove(TDelegate target)
        {
            if (target == null)
                return;
            foreach (Delegate d in (target as Delegate).GetInvocationList())
            {
                MethodTarget mt = targets.Find(w => Equals(d.Target, (w.Reference?.Target)) && Equals(d.Method.MethodHandle, w.Method.MethodHandle));
                if (mt != null)
                    targets.Remove(mt);
            }
        }
        public TDelegate Target
        {
            get
            {
                Delegate combinedTarget = null;
                foreach (MethodTarget mt in targets.ToArray())
                {
                    WeakReference wr = mt.Reference;
                    // Static target || alive instance target
                    if (wr == null || wr.Target != null)
                    {
                        var newDelegate = Delegate.CreateDelegate(typeof(TDelegate), wr?.Target, mt.Method);
                        combinedTarget = Delegate.Combine(combinedTarget, newDelegate);
                    }
                    else
                        targets.Remove(mt);
                }
                return combinedTarget as TDelegate;
            }
            set
            {
                targets.Clear();
                Combine(value);
            }
        }
    }



    public class Foo
    {
        WeakDelegate<EventHandler> _click = new WeakDelegate<EventHandler>();
        public event EventHandler Click
        {
            add { _click.Combine(value); }
            remove { _click.Remove(value); }
        }
        protected virtual void OnClick(EventArgs e)
            => _click.Target?.Invoke(this, e);
    }

}
