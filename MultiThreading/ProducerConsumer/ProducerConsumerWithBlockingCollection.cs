using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class ProducerConsumerWithBlockingCollection : IDisposable
    {
        BlockingCollection<Action> tasks = new BlockingCollection<Action>();
        public ProducerConsumerWithBlockingCollection(int consumerCount)
        {
            for (int i = 0; i < consumerCount; i++)
                Task.Factory.StartNew(Consume);
        }
        public void Dispose()
        {
            tasks.CompleteAdding();
        }
        public void EnqueueTask(Action action)
        {
            tasks.Add(action);
        }
        void Consume()
        {
            // This sequence that we’re enumerating will block when no elements
            // are available and will end when CompleteAdding is called.
            foreach (Action action in tasks.GetConsumingEnumerable())
                action();
        }
    }


    public class ProducerConsumerWithBlockingCollectionYieldingTask : IDisposable
    {
        class TaskItem
        {
            public readonly TaskCompletionSource<object> TaskSource;
            public readonly Action Action;
            public readonly CancellationToken? CancelToken;
            public TaskItem(TaskCompletionSource<object> taskSource, Action action, CancellationToken? cancelToken)
            {
                TaskSource = taskSource;
                Action = action;
                CancelToken = cancelToken;
            }
        }

        BlockingCollection<TaskItem> tasks = new BlockingCollection<TaskItem>();
        public ProducerConsumerWithBlockingCollectionYieldingTask(int consumerCount)
        {
            for (int i = 0; i < consumerCount; i++)
                Task.Factory.StartNew(Consume);
        }
        public void Dispose()
        {
            tasks.CompleteAdding();
        }
        public Task EnqueueTask(Action action)
        {
            return EnqueueTask(action, null);
        }
        public Task EnqueueTask(Action action, CancellationToken? cancelToken)
        {
            var tcs = new TaskCompletionSource<object>();
            tasks.Add(new TaskItem(tcs, action, cancelToken));
            return tcs.Task;
        }
        void Consume()
        {
            foreach (TaskItem workItem in tasks.GetConsumingEnumerable())
                if (workItem.CancelToken.HasValue && workItem.CancelToken.Value.IsCancellationRequested)
                {
                    workItem.TaskSource.SetCanceled();
                }
                else
                    try
                    {
                        workItem.Action();
                        workItem.TaskSource.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        workItem.TaskSource.SetException(ex);
                    }
        }
    }


}
