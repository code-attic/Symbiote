using System;
using System.Threading;

namespace Symbiote.Core.Utility
{
    public class RingBufferCell
        : IDisposable
    {
        public volatile object Value;
        public volatile int LastWriter;
        public RingBuffer Ring { get; set; }
        public RingBufferCell Next { get; set; }
        public bool Tail { get; set; }
        public bool Disposed { get; set; }
        public object Lock { get; set; }

        //public static RingBufferCell Build(RingBuffer ring, int count)
        //{
        //    var newCell = new RingBufferCell(ring);
        //    ring.Head = ring.Head ?? newCell;
        //    if ((newCell.Tail = (count == 0)))
        //        newCell.Next = ring.Head;
        //    else
        //        newCell.Next = Build(ring, count - 1);
        //    return newCell;
        //}

        public static RingBufferCell Build(RingBuffer ring, int count)
        {
            var head = new RingBufferCell( ring );
            var last = head;
            while(--count > 0)
            {
                var cell = new RingBufferCell(ring);
                last.Next = cell;
                last = cell;
            }
            last.Next = head;
            last.Tail = true;
            return head;
        }

        public bool Ready(int index)
        {
            return Ring.PreviousStepLookup[index] == LastWriter;
        }

        public RingBufferCell Transform(int index, Func<object, object> transform)
        {
            while (!Ready(index))
                Thread.Sleep(1);
            lock (Lock)
            {
                var value = transform(Value);
                Value = value;
                LastWriter = index;
            }
            return Next;
        }

        public RingBufferCell(RingBuffer ring)
        {
            Ring = ring;
            LastWriter = 0;
            Lock = new object();
        }

        public void Dispose()
        {
            Disposed = true;
            Next = null;
            Ring = null;
            Value = null;
        }
    }
}