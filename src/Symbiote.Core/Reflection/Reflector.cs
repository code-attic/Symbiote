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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Reflection
{
    public class Reflector
    {
        private static List<Type> initializedTypes = new List<Type>();

        private static
            ConcurrentDictionary<Tuple<Type, string>, Tuple<Type, Func<object, object>, Action<object, object>>>
            memberCache =
                new ConcurrentDictionary<Tuple<Type, string>, Tuple<Type, Func<object, object>, Action<object, object>>>
                    ();

        private static BindingFlags bindingFlags = BindingFlags.FlattenHierarchy |
                                                   BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Instance;

        private static BindingFlags propertyFlags = BindingFlags.FlattenHierarchy |
                                                    BindingFlags.Public |
                                                    BindingFlags.Instance;

        public static IEnumerable<Type> GetInheritanceChain( Type type )
        {
            Type baseType = type.BaseType;
            Type simpleObject = typeof( object );

            while ( baseType != null && baseType != simpleObject )
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }

            foreach( var baseInterface in type.GetInterfaces() )
            {
                yield return baseInterface;
            }
            yield break;
        }

        /// <summary>
        /// Finds all super-types that the given type descends from, including the type passed in
        /// </summary>
        /// <param name="type">The type to start at, when you want to find all super-types</param>
        /// <returns>list of types that the provided type inherits from, including the type argument passed in</returns>
        public static IEnumerable<Type> GetInheritanceChainFor( Type type )
        {
            yield return type;
            var chain = GetInheritanceChain( type );
            if ( chain != null )
            {
                foreach( var t in chain )
                {
                    yield return t;
                }
            }
            yield break;
        }

        /// <summary>
        /// Returns the types that inherit from the provided type.
        /// </summary>
        /// <param name="type">Super-type you want to find sub-types for.</param>
        /// <returns>list of types that inherit from the provided type.</returns>
        public static IEnumerable<Type> GetSubTypes( Type type )
        {
            yield return type;
            // Machine.Specifications and Moq-generated assemblies blow up when reading Types in, plus we don't need 'em.
            var children = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(
                    a =>
                    !a.FullName.Contains( "DynamicProxyGenAssembly2" ) &&
                    !a.FullName.Contains( "Machine.Specifications" ) )
                .SelectMany( s => s.GetTypes() )
                .Where( x => (x.IsSubclassOf( type ) || type.IsAssignableFrom( x )) && x != type )
                .ToList();
            foreach( var t in children.Distinct() )
            {
                yield return t;
            }
            yield break;
        }

        private static void CreateLookupsForType( Type type )
        {
            if ( initializedTypes.Contains( type ) )
                return;
            try
            {
                type
                    .GetMembers( bindingFlags )
                    .Where( x => x.MemberType == MemberTypes.Property || x.MemberType == MemberTypes.Field )
                    .ForEach( x =>
                                  {
                                      var key = Tuple.Create( type, x.Name );
                                      var memberType = GetMemberInfoType( x );

                                      var getter = CanRead( type, x.Name ) ? BuildGet( type, x.Name ) : null;
                                      var setter = CanWrite( type, x.Name ) ? BuildSet( type, x.Name ) : null;
                                      var value = Tuple.Create( memberType, getter, setter );

                                      memberCache.TryAdd( key, value );
                                  }
                    );
                initializedTypes.Add( type );
            }
            catch ( Exception e )
            {
                throw e;
            }
        }

        public static IEnumerable<Type> GetInterfaceChain( Type type )
        {
            return type.GetInterfaces();
        }

        private static Type GetMemberInfoType( MemberInfo memberInfo )
        {
            try
            {
                if ( memberInfo.MemberType == MemberTypes.Property )
                    return memberInfo.DeclaringType.GetProperty( memberInfo.Name, bindingFlags ).PropertyType;
                else
                    return memberInfo.DeclaringType.GetField( memberInfo.Name, bindingFlags ).FieldType;
            }
            catch ( Exception e )
            {
                return null;
            }
        }

        private static Func<object, object> BuildGet( Type type, string member )
        {
            var param = Expression.Parameter( typeof( object ), "container" );
            var func = Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.PropertyOrField(
                        Expression.Convert( param, type ), member ), typeof( object )
                    ),
                param );
            return func.Compile();
        }

        private static Action<object, object> BuildSet( Type type, string member )
        {
            var memberInfo = type.GetMember( member,
                                             BindingFlags.Public |
                                             BindingFlags.NonPublic |
                                             BindingFlags.Instance |
                                             BindingFlags.FlattenHierarchy ).First();
            if( CanWrite( memberInfo ) )
            {
                var memberType = GetMemberInfoType( memberInfo );
                var param1 = Expression.Parameter( typeof( object ), "container" );
                var param2 = Expression.Parameter( typeof( object ), "value" );

                var instanceConversion = Expression.Convert( param1, type );
                var propertyOrField = Expression.PropertyOrField( instanceConversion, member );
                var valueConversion = Expression.Convert( param2, memberType );
                var assignment = Expression.Assign( propertyOrField, valueConversion );

                var func = Expression.Lambda<Action<object, object>>( assignment, param1, param2 );
                return func.Compile();
            }
            return (x, y) => { };
        }

        public static Type GetMemberType( Type type, string memberName )
        {
            CreateLookupsForType( type );
            return memberCache[Tuple.Create( type, memberName )].Item1;
        }

        public static bool CanWrite( MemberInfo memberInfo )
        {
            if( memberInfo.MemberType == MemberTypes.Field )
                return true;
            else if( memberInfo.MemberType == MemberTypes.Property )
                return GetPropertyInfo( memberInfo.DeclaringType, memberInfo.Name ).CanWrite;
            return false;
        }

        public static FieldInfo GetFieldInfo( Type type, string field )
        {
            return type.GetField( field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy );
        }

        public static PropertyInfo GetPropertyInfo( Type type, string property )
        {
            return type.GetProperty( property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy );
        }

        public static IEnumerable<PropertyInfo> GetProperties( Type type )
        {
            return type.GetProperties( propertyFlags );
        }

        public static bool CanRead( Type type, string memberName )
        {
            var member = type.GetMember( memberName, bindingFlags ).First();
            if ( member.MemberType == MemberTypes.Field )
            {
                return true;
            }
            else if ( member.MemberType == MemberTypes.Property )
            {
                var property = type.GetProperty( memberName, bindingFlags );
                return property.CanRead;
            }
            return false;
        }

        public static bool CanWrite( Type type, string memberName )
        {
            var member = type.GetMember( memberName, bindingFlags ).First();
            if ( member.MemberType == MemberTypes.Field )
            {
                return true;
            }
            else if ( member.MemberType == MemberTypes.Property )
            {
                var property = type.GetProperty( memberName, bindingFlags );
                return property.CanWrite;
            }
            return false;
        }

        public static object ReadMember( object instance, string memberName )
        {
            try
            {
                var type = instance.GetType();
                CreateLookupsForType( type );
                var parts = memberName.Split( '.' );
                if ( parts.Length > 1 )
                {
                    var parent = ReadFromCache( instance, type, parts[0] );
                    if ( parent != null )
                        return ReadMember( parent, string.Join( ".", parts.Skip( 1 ) ) );
                    else
                        return null;
                }
                else
                {
                    return ReadFromCache( instance, type, memberName );
                }
            }
            catch ( Exception e )
            {
                return null;
            }
        }

        protected static object ReadFromCache( object instance, Type type, string memberName )
        {
            var tuple = memberCache[Tuple.Create( type, memberName )];
            if ( tuple.Item2 != null )
                return tuple.Item2.Invoke( instance );
            return null;
        }

        public static T ReadMember<T>( object instance, string memberName )
        {
            try
            {
                var type = instance.GetType();
                CreateLookupsForType( type );
                var parts = memberName.Split( '.' );
                if ( parts.Length > 1 )
                {
                    var parent = ReadFromCache( instance, type, parts[0] );
                    if ( parent != null )
                        return ReadMember<T>( parent, string.Join( ".", parts.Skip( 1 ) ) );
                    else
                        return default(T);
                }
                else
                {
                    return ReadFromCache<T>( instance, type, memberName );
                }
            }
            catch ( Exception e )
            {
                return default(T);
            }
        }

        protected static T ReadFromCache<T>( object instance, Type type, string memberName )
        {
            var tuple = memberCache[Tuple.Create( type, memberName )];
            if ( tuple.Item2 != null )
                return (T) tuple.Item2.Invoke( instance );
            return default(T);
        }

        public static void WriteMember( object instance, string memberName, object value )
        {
            try
            {
                var type = instance.GetType();
                CreateLookupsForType( type );
                var parts = memberName.Split( '.' );
                if ( parts.Length > 1 )
                {
                    var parent = ReadFromCache( instance, type, parts[0] );
                    if ( parent == null )
                    {
                        var intended = GetMemberType( type, parts[0] );
                        parent = Assimilate.GetInstanceOf( intended );
                    }
                    WriteMember( parent, string.Join( ".", parts.Skip( 1 ) ), value );
                }
                else
                    WriteThroughCache( instance, type, memberName, value );
            }
            catch ( Exception e )
            {
                // do nothing
            }
        }

        protected static void WriteThroughCache( object instance, Type type, string memberName, object value )
        {
            var tuple = memberCache[Tuple.Create( type, memberName )];
            if ( tuple.Item3 != null )
                tuple.Item3.Invoke( instance, value );
        }
    }
}