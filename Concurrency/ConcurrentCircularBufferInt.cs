using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency
{
    public sealed class ConcurrentCircularBufferInt : ConcurrentCircularBuffer<int>
    {
        #region Delegates
        public event Action<int> ValueChanged;
        #endregion

        #region Properties
        public int sum { get; set; }
        #endregion

        #region Constructor
        public ConcurrentCircularBufferInt(int maxSizeInput = int.MaxValue) : base(maxSizeInput)
        {
            sum = 0;
        }

        public override void enqueueWithoutRemoval(int input)
        {
            base.enqueueWithoutRemoval(input);
            sum += input;
            ValueChanged?.Invoke(sum);
        }

        public override void enqueueWithRemoval(int input)
        {
            base.enqueueWithRemoval(input);
            sum = sum + input - _removeObject;
            ValueChanged?.Invoke(sum);
        }
        #endregion
    }
}
