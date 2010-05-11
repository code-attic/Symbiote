using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Symbiote.Core.Reflection
{
    public static class ReflectionExtensions
    {
        public static object GetMemberValue(this MemberInfo member, object source)
        {
            if(member.MemberType == MemberTypes.Field)
            {
                
            }
            else if(member.MemberType == MemberTypes.Property)
            {
                
            }
        }
    }

    public class Reflector
    {
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
    }
}
