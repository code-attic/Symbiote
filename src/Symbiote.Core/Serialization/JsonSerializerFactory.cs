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
using System;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Symbiote.Core.Serialization
{
    public class JsonSerializerFactory : IJsonSerializerFactory
    {
        private readonly string _resolverFormat = "CustomJsonResolver-{0}";

        public JsonSerializer GetSerializerFor( string json, bool includeTypeSpec )
        {
            var type = json.GetSerializedTypeFromJson();
            return GetSerializerFor( type, includeTypeSpec );
        }

        public JsonSerializer GetSerializerFor<T>( bool includeTypeSpec )
        {
            var type = typeof( T );
            return GetSerializerFor( type, includeTypeSpec );
        }

        public JsonSerializer GetSerializerFor( Type type, bool includeTypeSpec )
        {
            return JsonSerializer.Create( includeTypeSpec ? GetDefaultSettings() : GetSettingsWithoutTypeHandling() );
        }

        protected JsonSerializerSettings GetDefaultSettings()
        {
            var settings = new JsonSerializerSettings
                               {
                                   NullValueHandling = NullValueHandling.Ignore,
                                   MissingMemberHandling = MissingMemberHandling.Ignore,
                                   TypeNameHandling = TypeNameHandling.All,
                                   TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                               };
            settings.Converters.Add( new IsoDateTimeConverter() );
            return settings;
        }

        protected JsonSerializerSettings GetSettingsWithoutTypeHandling()
        {
            var settings = GetDefaultSettings();
            settings.TypeNameHandling = TypeNameHandling.None;
            return settings;
        }
    }
}