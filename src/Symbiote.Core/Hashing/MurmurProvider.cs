using System;
using System.Text;

namespace Symbiote.Core.Hashing
{
    public class MurmurProvider : IHashingProvider
    {
        public MurmurHash3 _hasher;
        public MurmurHash3 Hasher
        {
            get 
            {
                _hasher = _hasher ?? new MurmurHash3();
                return _hasher;
            }
        }

        private uint Murmolade( byte[] bytes )
        {
            return Hasher.Hash( bytes, bytes.Length, 0xc58f1a7b );
        }

        private ulong Murmolade64( byte[] bytes )
        {
            return Hasher.Hash64( bytes, bytes.Length, 0xc58f1a7b );
        }

        public uint Murmur( string value )
        {
            return Murmolade(Encoding.UTF8.GetBytes( value ) );
        }

        public ulong Murmur64( string value )
        {
            return Murmolade64(Encoding.UTF8.GetBytes( value ) );
        }

        public long Hash<T>( T value )
        {
            var underlying = value.ToString();
            return (long) Murmur64( underlying );
        }
    }
}