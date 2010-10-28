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

using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;

namespace Symbiote.Messaging.Impl.Serialization
{
    public class JsonMessageSerializer : IMessageSerializer
    {
        public T Deserialize<T>(byte[] message)
        {
            var json = Encoding.UTF8.GetString(message);
            return json.FromJson<T>();
        }

        public object Deserialize(byte[] message)
        {
            var json = Encoding.UTF8.GetString(message);
            return json.FromJson();
        }

        public byte[] Serialize<T>(T body)
        {
            var json = body.ToJson();
            return Encoding.UTF8.GetBytes( json );
        }
    }

}
