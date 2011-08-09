using System.Text;
using Symbiote.Core.Hashing;

namespace Core.Tests.Hashing
{
    public static class MurmurExtensions
    {
        public static MurmurHash3 _hasher;
        public static MurmurHash3 Hasher
        {
            get 
            {
                _hasher = _hasher ?? new MurmurHash3();
                return _hasher;
            }
        }

        private static uint Murmolade( byte[] bytes )
        {
            return Hasher.Hash( bytes, bytes.Length, 0xc58f1a7b );
        }

        private static ulong Murmolade64( byte[] bytes )
        {
            return Hasher.Hash64( bytes, bytes.Length, 0xc58f1a7b );
        }

        public static uint Murmur( this string value )
        {
            return Murmolade(Encoding.UTF8.GetBytes( value ) );
        }

        public static ulong Murmur64( this string value )
        {
            return Murmolade64(Encoding.UTF8.GetBytes( value ) );
        }
    }
}