using System;
using System.Security.Cryptography;
using System.Text;

namespace Symbiote.Core.Hashing
{
    public class MD5HashProvider
        : IHashingProvider
    {
        protected MD5 Provider { get; set; }

        public long Hash<T>(T value)
        {
            var temp = value.ToString();
            var hashBytes = Provider.ComputeHash(Encoding.ASCII.GetBytes(temp));
            return BitConverter.ToInt64(hashBytes, 0) + BitConverter.ToInt64(hashBytes, 8);
        }

        public MD5HashProvider()
        {
            Provider = MD5.Create();
        }
    }
}