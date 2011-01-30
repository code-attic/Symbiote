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
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Symbiote.Core.Serialization
{
    public static class SerializationExtensions
    {
        public static bool IsBinarySerializable( this Type type )
        {
            return
                type.HasDefaultConstructor() &&
                type.GetCustomAttributes( typeof( SerializableAttribute ), true ).Length == 1;
        }

        public static bool HasDefaultConstructor( this Type type )
        {
            return
                type
                    .GetConstructors()
                    .Any( x => x.IsPublic && !x.IsStatic && x.GetParameters().Length == 0 );
        }

        public static bool IsProtobufSerializable( this Type type )
        {
            return type.HasDefaultConstructor() && type.MarkedWithDataContracts();
        }

        public static bool IsJsonSerializable( this Type type )
        {
            return type.HasDefaultConstructor() && type.ReadOnlyPropertiesMarkedWithJsonIgnore();
        }

        public static bool MarkedWithDataContracts( this Type type )
        {
            var hasDataContract = type.GetCustomAttributes( typeof( DataContractAttribute ), true ).Length == 1;
            var hasMemberContractsOnProperties =
                type.GetProperties(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance )
                    .All( x => x.GetCustomAttributes( typeof( DataMemberAttribute ), true ).Length == 1 );

            var hasMemberContractsOnFields =
                type.GetFields(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance )
                    .All( x => x.GetCustomAttributes( typeof( DataMemberAttribute ), true ).Length == 1 );
            return hasDataContract && hasMemberContractsOnFields && hasMemberContractsOnProperties;
        }

        public static bool ReadOnlyPropertiesMarkedWithJsonIgnore( this Type type )
        {
            var hasIgnoreOnReadOnlyProperties =
                type.GetProperties(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance )
                    .Where( x => !x.CanWrite )
                    .All( x => x.GetCustomAttributes( typeof( JsonIgnoreAttribute ), true ).Length == 1 );

            var hasIgnoreOnReadOnlyFields =
                type.GetFields(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance )
                    .Where( x => x.IsInitOnly || x.IsLiteral )
                    .All( x => x.GetCustomAttributes( typeof( JsonIgnoreAttribute ), true ).Length == 1 );
            return hasIgnoreOnReadOnlyFields && hasIgnoreOnReadOnlyProperties;
        }
    }
}