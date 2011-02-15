// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System.IO;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json.Converters;
using Symbiote.Couch.Impl.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ServiceStack.Text;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Symbiote.Couch.Impl.Metadata 
{
    public class SerializationStrategy :
        ISerializeDocument
    {
        public IContractResolver PocoContractResolver {get; set; }
        public JsonSerializer DocumentSerializer { get; set; }
        public JsonSerializer PocoSerializer { get; set; }

        public void Serialize<T>( T instance, TextWriter writer )
        {
            if( !typeof( T ).IsInstanceOf( typeof( BaseDocument ) ) )
            {
                PocoSerializer.Serialize( writer, instance );
            }
            else
            {
                DocumentSerializer.Serialize( writer, instance );
            }
        }

        public T Deserialize<T>( string json )
        {
            using( var reader = new StringReader( json ))
            {
                if( !typeof( T ).IsInstanceOf( typeof( BaseDocument ) ) )
                {
                    return (T) PocoSerializer.Deserialize( reader, typeof( T ) );
                }
                else
                {
                    return (T) DocumentSerializer.Deserialize( reader, typeof( T ) );
                }
            }
        }

        public SerializationStrategy( DocumentContractResolver contractResolver )
        {
            PocoContractResolver = contractResolver;
            DocumentSerializer = JsonSerializer.Create( GetDocumentSettings() );
            PocoSerializer = JsonSerializer.Create( GetPocoSettings() );
        }

        public JsonSerializerSettings GetPocoSettings()
        {
            var settings = new JsonSerializerSettings
                               {
                                   NullValueHandling = NullValueHandling.Ignore,
                                   MissingMemberHandling = MissingMemberHandling.Ignore,
                                   TypeNameHandling = TypeNameHandling.All,
                                   TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                                   ContractResolver = PocoContractResolver
                               };
            settings.Converters.Add( new IsoDateTimeConverter() );
            return settings;
        }

        public JsonSerializerSettings GetDocumentSettings()
        {
            var settings = new JsonSerializerSettings
                               {
                                   NullValueHandling = NullValueHandling.Ignore,
                                   MissingMemberHandling = MissingMemberHandling.Ignore,
                                   TypeNameHandling = TypeNameHandling.All,
                                   TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                               };
            settings.Converters.Add( new IsoDateTimeConverter() );
            return settings;
        }
    }
}
