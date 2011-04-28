using Machine.Specifications;

namespace Core.Tests.Utility.RingBuffer
{
    public class with_buffer
    {
        public static int Size = 10000;
        public static Symbiote.Core.Concurrency.RingBuffer Buffer { get; set; }
        private Establish context = () =>
        {
            Buffer = new Symbiote.Core.Concurrency.RingBuffer(Size);
        };
    }
}