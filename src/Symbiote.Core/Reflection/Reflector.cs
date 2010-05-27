using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

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
            return type.GetInterface("IEnumerable") != null;
        }

        public static Type DetermineElementType(this IEnumerable enumerable)
        {
            return typeof (object);
        }
    }

    public class Reflector
    {
        //static ConcurrentDictionary<Tuple<Type, string>, Tuple<Type, Getter, Setter>>
        //    reflectorCache = new ConcurrentDictionary<Tuple<Type, string>, Tuple<Type, Getter, Setter>>();

        public static IEnumerable<Type> GetInheritenceChain(Type type)
        {
            var baseType = type.BaseType;
            if (baseType == null || baseType == typeof(object))
                return null;

            var types = new[] { baseType };
            var enumerable = GetInheritenceChain(baseType);
            return enumerable == null ? types : types.Concat(enumerable);
        }

        public static IEnumerable<Type> GetInterfaceChain(Type type)
        {
            return type.GetInterfaces();
        }

        public static Type GetMemberType(MemberInfo memberInfo)
        {
            if(memberInfo.MemberType == MemberTypes.Field)
            {
                
            }
            else if(memberInfo.MemberType == MemberTypes.Property)
            {
                
            }
            return null;
        }

        public static Type GetMemberType(Type type, string memberPath)
        {
            return null;
        }
    }
}
