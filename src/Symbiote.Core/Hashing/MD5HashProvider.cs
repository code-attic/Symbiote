using System;
using System.Security.Cryptography;
using System.Text;

namespace Symbiote.Core.Hashing
{
    public class MD5HashProvider
        : IHashingProvider
    {
        protected MD5 Provider { get; set; }

        public int Hash<T>(T value)
        {
            var temp = value.ToString();
            var hashBytes = Provider.ComputeHash(Encoding.UTF8.GetBytes(temp));
            return BitConverter.ToInt32(hashBytes, 0);
        }

        public MD5HashProvider()
        {
            Provider = MD5.Create();
        }
    }
}