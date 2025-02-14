﻿﻿// Net 4.X has ConcurrentQueue, but ConcurrentQueue has no TryDequeueAll method,
// which makes SafeQueue twice as fast for the send thread.
//
// uMMORPG 450 CCU
//   SafeQueue:       900-1440ms latency
//   ConcurrentQueue:     2000ms latency
//
// It's also noticeable in the LoadTest project, which hardly handles 300 CCU with ConcurrentQueue!

 using System.Collections.Generic;

 namespace Spring.Collection
{
    public class SafeQueue<T>
    {
        readonly Queue<T> queue = new Queue<T>();

        // for statistics. don't call Count and assume that it's the same after the call.
        public int Count
        {
            get
            {
                lock (queue)
                {
                    return queue.Count;
                }
            }
        }

        public void Enqueue(T item)
        {
            lock (queue)
            {
                queue.Enqueue(item);
            }
        }
        
        public T Dequeue()
        {
            lock (queue)
            {
                if (queue.Count == 0)
                {
                    return default(T);
                }
                return queue.Dequeue();
            }
        }


        // for when we want to dequeue and remove all of them at once without
        // locking every single TryDequeue.
        public bool TryDequeueAll(out T[] result)
        {
            lock (queue)
            {
                result = queue.ToArray();
                queue.Clear();
                return result.Length > 0;
            }
        }

        public void Clear()
        {
            lock (queue)
            {
                queue.Clear();
            }
        }
    }
}
