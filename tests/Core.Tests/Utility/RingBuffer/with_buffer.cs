using Machine.Specifications;
using Symbiote.Core.Utility;

namespace ringbuffer
{
    public class with_buffer
    {
        public static int Size = 10000;
        public static RingBuffer Buffer { get; set; }
        private Establish context = () =>
        {
            Buffer = new RingBuffer(Size);
        };
    }
}