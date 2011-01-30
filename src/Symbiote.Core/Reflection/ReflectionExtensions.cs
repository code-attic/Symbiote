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
using System.Collections;
using System.Reflection;

namespace Symbiote.Core.Reflection
{
    public static class ReflectionExtensions
    {
        private static readonly BindingFlags _bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic |
                                                             BindingFlags.Public | BindingFlags.Instance;

        public static object GetMemberValue( this MemberInfo member, object source )
        {
            if ( member.MemberType == MemberTypes.Field )
            {
                return source.GetType().GetField( member.Name, _bindingFlags ).GetValue( source );
            }
            else if ( member.MemberType == MemberTypes.Property )
            {
                return source.GetType().GetProperty( member.Name, _bindingFlags ).GetValue( source, null );
            }
            return null;
        }

        public static bool IsEnumerable( this Type type )
        {
            return type.GetInterface( "IEnumerable", false ) != null;
        }

        public static Type DetermineElementType( this IEnumerable enumerable )
        {
            return typeof( object );
        }
    }
}