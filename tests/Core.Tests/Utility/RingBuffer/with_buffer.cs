using Machine.Specifications;

namespace Core.Tests.Utility.RingBuffer
{
    public class with_buffer
    {
        public static int Size = 10000;
        public static Symbiote.Core.Utility.RingBuffer Buffer { get; set; }
        private Establish context = () =>
        {
            Buffer = new Symbiote.Core.Utility.RingBuffer(Size);
        };
    }
}