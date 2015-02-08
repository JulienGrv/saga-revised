using System;
using System.Collections.Generic;
using System.Threading;

namespace Saga.Map.Utils.Definitions.Misc
{
    public class Pool<T>
    {
        public Pool(int count)
        {
            pending = new Queue<T>(count);
        }

        private Queue<T> pending = new Queue<T>(1);

        public T Request()
        {
            lock (pending)
            {
                int tick = Environment.TickCount;
                while (pending.Count == 0)
                {
                    if (Environment.TickCount - tick > 8000)
                    {
                        System.Diagnostics.Trace.Fail("Database pool error");
                        throw new SystemException("Database pool error");
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                return pending.Dequeue();
            }
        }

        public void Release(T item)
        {
            lock (pending)
            {
                pending.Enqueue(item);
            }
        }
    }
}