namespace Symbiote.Core.Impl.Utility
{
    public struct RingBufferIndex
    {
        public int Initial;
        public int Value;
        public int Limit;

        public RingBufferIndex( int initial, int limit )
        {
            Value = Initial = initial;
            Limit = limit;
        }

        public static implicit operator int(RingBufferIndex index)
        {
            return index.Value;
        }

        public static RingBufferIndex operator +(RingBufferIndex index, int qty)
        {
            var total = index.Value + qty;
            index.Value = total > index.Limit
                              ? index.Initial + ( total - index.Limit )
                              : total;
            return index;
        }

        public static RingBufferIndex operator -(RingBufferIndex index, int qty)
        {
            var total = index.Value - qty;
            index.Value = total > index.Initial
                              ? index.Limit + (total + index.Initial)
                              : total;
            return index;
        }

        public static RingBufferIndex operator ++(RingBufferIndex index)
        {
            index.Value = index.Value == index.Limit
                              ? index.Initial
                              : index.Value + 1;
            return index;
        }
    }
}