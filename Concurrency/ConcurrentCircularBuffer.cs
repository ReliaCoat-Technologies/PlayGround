using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Concurrency
{
    public class ConcurrentCircularBuffer
    {
        private static object lockObj = new object();

        public event Action<int> ValueChanged;
        public AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public ConcurrentQueue<int> buffer { get; set; }
        public int sum { get; set; }
        public int count { get; set; }
        public int maxSize { get; set; }

        public ConcurrentCircularBuffer(int maxSizeInput)
        {
            buffer = new ConcurrentQueue<int>();
            maxSize = maxSizeInput;
            count = 0;
        }

        public void addToQueue(int item)
        {
            Task.Run(() =>
            {
                count++;
                if (count <= maxSize)
                {
                    sum += item;
                    buffer.Enqueue(item);
                }
                else
                {
                    buffer.Enqueue(item);
                    int itemToRemove;
                    var success = buffer.TryDequeue(out itemToRemove);
                    if (success) sum += item - itemToRemove;
                    else sum += item;
                    count--;
                }
                autoResetEvent.Set();
            });
            ValueChanged?.Invoke(sum);
            autoResetEvent.WaitOne();
        }
    }
}
