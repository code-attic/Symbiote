using System;

namespace Symbiote.Http.Owin.Impl
{
    public class ConsumableSegment<T>
    {
        private int _count;
        private int _length;
        private int _offset;
        
        public T[] Array { get; set; }

        public int Offset 
        {
            get { return _offset; }
            set 
            {
                SetOffset( value );
            }
        }

        public int Count 
        {  
            get { return _count; }
        }

        public int Length
        {
            get { return _length; }
        }

        private bool SetOffset( int value )
        {
            if( value < _length && value >= 0 )
            {
                _offset = value;
                _count = _length - _offset - 1;
                return true;
            }
            return false;
        }

        public bool Next()
        {
            var test = _offset + 1;
            return SetOffset( test );
        }

        public ConsumableSegment( T[] originalArray ) : this ( originalArray, 0, originalArray.Length )
        {
        }

        public ConsumableSegment( T[] originalArray, int offset, int count )
        {
            Array = originalArray;
            _offset = offset;
            _length = count;
            _count = count;
        }

        public static implicit operator ConsumableSegment<T>( ArraySegment<T> arraySegment )
        {
            return new ConsumableSegment<T>( arraySegment.Array, arraySegment.Offset, arraySegment.Count );
        }

        public static implicit operator ArraySegment<T>( ConsumableSegment<T> consumableSegment )
        {
            return new ArraySegment<T>( 
                consumableSegment.Array, 
                consumableSegment.Offset, 
                consumableSegment.Count + 1 );
        }
    }
}