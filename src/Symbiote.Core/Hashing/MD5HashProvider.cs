/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Security.Cryptography;
using System.Text;

namespace Symbiote.Core.Hashing
{
    public class MD5HashProvider
        : IHashingProvider
    {
        protected MD5 Provider { get; set; }
        protected object _lock = new object();

        public long Hash<T>(T value)
        {
            var temp = value.ToString();
            byte[] hashBytes;
            lock(_lock)
            {
                hashBytes = Provider.ComputeHash(Encoding.ASCII.GetBytes(temp));
            }
            return BitConverter.ToInt64(hashBytes, 0) + BitConverter.ToInt64(hashBytes, 8);
        }

        public MD5HashProvider()
        {
            Provider = MD5.Create();
        }
    }
}