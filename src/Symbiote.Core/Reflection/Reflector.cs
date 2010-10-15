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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Reflection
{
    public static class ReflectionExtensions
    {
        private static readonly BindingFlags _bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        public static object GetMemberValue(this MemberInfo member, object source)
        {
            if(member.MemberType == MemberTypes.Field)
            {
                return source.GetType().GetField(member.Name, _bindingFlags).GetValue(source);
            }
            else if(member.MemberType == MemberTypes.Property)
            {
                return source.GetType().GetProperty(member.Name, _bindingFlags).GetValue(source, null);
            }
            return null;
        }
    
        public static bool IsEnumerable(this Type type)
        {
            return type.GetInterface("IEnumerable", false) != null;
        }

        public static Type DetermineElementType(this IEnumerable enumerable)
        {
            return typeof (object);
        }
    }

    public class Reflector
    {
        private static List<Type> initializedTypes = new List<Type>();
        static ConcurrentDictionary<Tuple<Type, string>, Tuple<Type, Func<object, object>, Action<object, object>>> memberCache =
            new ConcurrentDictionary<Tuple<Type, string>, Tuple<Type, Func<object, object>, Action<object, object>>>();

        static BindingFlags bindingFlags = BindingFlags.FlattenHierarchy |
                                                   BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Instance;

        static BindingFlags propertyFlags = BindingFlags.FlattenHierarchy |
                                                   BindingFlags.Public |
                                                   BindingFlags.Instance;

        public static IEnumerable<Type> GetInheritenceChain(Type type)
        {
            var baseType = type.BaseType;
            if (baseType == null || baseType == typeof(object))
                return null;

            var types = new[] { baseType };
            var enumerable = GetInheritenceChain(baseType);
            return enumerable == null ? types : types.Concat(enumerable);
        }

        static void CreateLookupsForType(Type type)
        {
            if (initializedTypes.Contains(type))
                return;
            try
            {
                type
                    .GetMembers(bindingFlags)
                    .Where(x => x.MemberType == MemberTypes.Property || x.MemberType == MemberTypes.Field)
                    .ForEach(x =>
                                 {
                                     var key = Tuple.Create(type, x.Name);
                                     var memberType = GetMemberInfoType(x);
                                     
                                     var getter = CanRead(type, x.Name) ? BuildGet(type, x.Name) : null;
                                     var setter = CanWrite(type, x.Name) ? BuildSet(type, x.Name) : null;
                                     var value = Tuple.Create(memberType, getter, setter);

                                     memberCache.TryAdd(key, value);
                                 }
                    );
               initializedTypes.Add(type);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static IEnumerable<Type> GetInterfaceChain(Type type)
        {
            return type.GetInterfaces();
        }

        static Type GetMemberInfoType(MemberInfo memberInfo)
        {
            try
            {
                if (memberInfo.MemberType == MemberTypes.Property)
                    return memberInfo.DeclaringType.GetProperty(memberInfo.Name, bindingFlags).PropertyType;
                else
                    return memberInfo.DeclaringType.GetField(memberInfo.Name, bindingFlags).FieldType;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        static Func<object, object> BuildGet(Type type, string member)
        {
            var param = Expression.Parameter(typeof(object), "container");
            var func = Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.PropertyOrField(
                        Expression.Convert(param, type), member), typeof(object)
                    ),
                param);
            return func.Compile();
        }

        static Action<object, object> BuildSet(Type type, string member)
        {
            var memberInfo = type.GetMember(member,
                                            BindingFlags.Public |
                                            BindingFlags.NonPublic |
                                            BindingFlags.Instance |
                                            BindingFlags.FlattenHierarchy).First();
            var memberType = GetMemberInfoType(memberInfo);
            var param1 = Expression.Parameter(typeof(object), "container");
            var param2 = Expression.Parameter(typeof(object), "value");

            var instanceConversion = Expression.Convert(param1, type);
            var propertyOrField = Expression.PropertyOrField(instanceConversion, member);
            var valueConversion = Expression.Convert(param2, memberType);
            var assignment = Expression.Assign(propertyOrField, valueConversion);

            var func = Expression.Lambda<Action<object, object>>(assignment, param1, param2);
            return func.Compile();
        }

        public static Type GetMemberType(Type type, string memberName)
        {
            CreateLookupsForType(type);
            return memberCache[Tuple.Create(type, memberName)].Item1;
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(propertyFlags);
        }

        public static bool CanRead(Type type, string memberName)
        {
            var member = type.GetMember(memberName, bindingFlags).First();
            if(member.MemberType == MemberTypes.Field)
            {
                return true;
            }
            else if(member.MemberType == MemberTypes.Property)
            {
                var property = type.GetProperty(memberName, bindingFlags);
                return property.CanRead;
            }
            return false;
        }

        public static bool CanWrite(Type type, string memberName)
        {
            var member = type.GetMember(memberName, bindingFlags).First();
            if (member.MemberType == MemberTypes.Field)
            {
                return true;
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                var property = type.GetProperty(memberName, bindingFlags);
                return property.CanWrite;
            }
            return false;
        }

        public static object ReadMember(object instance, string memberName)
        {
            try
            {
                var type = instance.GetType();
                CreateLookupsForType(type);
                var tuple = memberCache[Tuple.Create(type, memberName)];
                if (tuple.Item2 != null)
                    return tuple.Item2.Invoke(instance);
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static T ReadMember<T>(object instance, string memberName)
        {
            try
            {
                var type = instance.GetType();
                CreateLookupsForType(type);
                var tuple = memberCache[Tuple.Create(type, memberName)];
                if(tuple.Item2 != null)
                    return (T) tuple.Item2.Invoke(instance);
                return default(T);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        public static void WriteMember(object instance, string memberName, object value)
        {
            try
            {
                var type = instance.GetType();
                CreateLookupsForType(type);
                var tuple = memberCache[Tuple.Create(type, memberName)];
                if (tuple.Item3 != null)
                    tuple.Item3.Invoke(instance, value);
            }
            catch (Exception e)
            {
                // do nothing
            }
        }
    }
}
