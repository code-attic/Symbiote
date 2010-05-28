using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Machine.Specifications;

namespace Core.Tests.Reflection.POC
{
    public class with_precompiled_read_cache
    {
        protected static Dictionary<Tuple<Type, string>, Tuple<Type, Func<object,object>>> readCache;
        protected static Dictionary<Tuple<Type, string>, Tuple<Type, Action<object,object>>> writeCache;
        protected static Stopwatch readCacheWatch;
        protected static Stopwatch writeCacheWatch;

        private static BindingFlags bindingFlags = BindingFlags.FlattenHierarchy |
                                                   BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Instance;

        private Establish context = () =>
                                        {
                                            readCacheWatch = Stopwatch.StartNew();
                                            readCache =
                                                typeof (TestClass)
                                                    .GetMembers(bindingFlags)
                                                    .Where(x => 
                                                           x.MemberType == MemberTypes.Property 
                                                           || x.MemberType == MemberTypes.Field)
                                                    .ToDictionary(
                                                        k => Tuple.Create(typeof(TestClass), k.Name),
                                                        v => Tuple.Create(GetMemberInfoType(v), BuildGet(typeof(TestClass), v.Name))
                                                    );
                                            readCacheWatch.Stop();

                                            writeCacheWatch = Stopwatch.StartNew();
                                            writeCache =
                                                typeof(TestClass)
                                                    .GetMembers(bindingFlags)
                                                    .Where(x => 
                                                           x.MemberType == MemberTypes.Property 
                                                           || x.MemberType == MemberTypes.Field)
                                                    .ToDictionary(
                                                        k => Tuple.Create(typeof(TestClass), k.Name),
                                                        v => Tuple.Create(GetMemberInfoType(v), BuildSet(typeof(TestClass), v.Name))
                                                    );
                                            writeCacheWatch.Stop();
                                        };

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

        static Func<object,object> BuildGet(Type type, string member)
        {
            var param = Expression.Parameter(typeof(object), "container");
            var func = Expression.Lambda<Func<object,object>>(
                Expression.Convert(
                    Expression.PropertyOrField(
                        Expression.Convert(param, type), member), typeof(object)
                    ),
                param);
            return func.Compile();
        }

        static Action<object,object> BuildSet(Type type, string member)
        {
            var memberInfo = type.GetMember(member,
                                            BindingFlags.Public | 
                                            BindingFlags.NonPublic | 
                                            BindingFlags.Instance |
                                            BindingFlags.FlattenHierarchy).First();
            var memberType = GetMemberInfoType(memberInfo);
            var param1 = Expression.Parameter(typeof(object), "container");
            var param2 = Expression.Parameter(typeof (object), "value");

            var instanceConversion = Expression.Convert(param1, type);
            var propertyOrField = Expression.PropertyOrField(instanceConversion, member);
            var valueConversion = Expression.Convert(param2, memberType);
            var assignment = Expression.Assign(propertyOrField,  valueConversion);
            
            var func = Expression.Lambda<Action<object, object>>(assignment, param1, param2);
            return func.Compile();
        }
    }
}
