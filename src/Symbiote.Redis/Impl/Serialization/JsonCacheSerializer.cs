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

using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Symbiote.Redis.Impl.Serialization
{
    public class JsonCacheSerializer
        : ICacheSerializer
    {
        protected JsonSerializerSettings GetDefaultSettings()
        {
            var settings = new JsonSerializerSettings()
                               {
                                   NullValueHandling = NullValueHandling.Ignore,
                                   MissingMemberHandling = MissingMemberHandling.Ignore,
                                   TypeNameHandling = TypeNameHandling.All,
                                   TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                               };
            settings.Converters.Add(new IsoDateTimeConverter());
            return settings;
        }

        public byte[] Serialize<T>(T value)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.None, GetDefaultSettings());
            return UTF8Encoding.UTF8.GetBytes(json);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            var json = UTF8Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json, GetDefaultSettings());
        }
    }
}