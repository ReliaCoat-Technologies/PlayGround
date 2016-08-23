using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency
{
    public abstract class ConcurrentCircularBuffer<T>
    {
        #region Statics
        private static object lockObject = new object();
        #endregion

        #region Fields
        private readonly int _maxSize;
        private int _currentSize;
        protected T _removeObject;
        #endregion

        #region Properties
        public ConcurrentQueue<T> buffer { get; set; }
        #endregion

        #region Constructor
        protected ConcurrentCircularBuffer(int maxSizeInput = int.MaxValue)
        {
            _currentSize = 0;
            _maxSize = maxSizeInput;
            buffer = new ConcurrentQueue<T>();
        }
        #endregion

        #region Methods
        public int enqueue(T input)
        {
            if (_currentSize <= _maxSize) enqueueWithoutRemoval(input);
            else enqueueWithRemoval(input);
            return _currentSize;
        }

        public int tryDequeue(out T itemToRemove)
        {
            var isRemoved = buffer.TryDequeue(out itemToRemove);
            if (isRemoved) _currentSize--;
            return _currentSize;
        }

        public virtual void enqueueWithoutRemoval(T input)
        {
            _currentSize++;
            buffer.Enqueue(input);
        }

        public virtual void enqueueWithRemoval(T input)
        {
            lock (lockObject)
            {
                _removeObject = default(T);
                var isRemoved = buffer.TryDequeue(out _removeObject);
                if(isRemoved) buffer.Enqueue(input);
            }
        }
        #endregion
    }
}
