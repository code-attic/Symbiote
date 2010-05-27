using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Machine.Specifications;

namespace Core.Tests.Reflection
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

    public class TestClass
    {
        internal string val1 = "1";
        protected int val2 = 2;
        private TimeSpan val3 = TimeSpan.FromSeconds(3);
        public Guid val4 = new Guid("00000000-0000-0000-0000-000000000004");

        public int GetVal2()
        {
            return val2;
        }

        public TimeSpan GetVal3()
        {
            return val3;
        }
    }


    public class when_reading_dynamically : with_precompiled_read_cache
    {
        protected static TestClass target = new TestClass();
        protected static string val1;
        protected static int val2;
        protected static TimeSpan val3;
        protected static Guid val4;
        protected static Stopwatch assignmentWatch;

        private Because of = () =>
                                 {
                                     assignmentWatch = Stopwatch.StartNew();

                                     val1 = readCache[Tuple.Create(typeof (TestClass), "val1")].Item2(target) as string;
                                     val2 = (int) readCache[Tuple.Create(typeof (TestClass), "val2")].Item2(target);
                                     val3 = (TimeSpan) readCache[Tuple.Create(typeof (TestClass), "val3")].Item2(target);
                                     val4 = (Guid) readCache[Tuple.Create(typeof (TestClass), "val4")].Item2(target);

                                     assignmentWatch.Stop();
                                     var ms = assignmentWatch.ElapsedMilliseconds;
                                 };

        private It should_match_val1 = () => val1.ShouldEqual(target.val1);
        private It should_match_val2 = () => val2.ShouldEqual(2);
        private It should_match_val3 = () => val3.ShouldEqual(TimeSpan.FromSeconds(3));
        private It should_match_val4 = () => val4.ShouldEqual(target.val4);
    }

    public class when_writing_dynamically : with_precompiled_read_cache
    {
        protected static TestClass target = new TestClass();
        protected static string val1;
        protected static int val2;
        protected static TimeSpan val3;
        protected static Guid val4;
        protected static Stopwatch assignmentWatch;

        private Because of = () =>
        {
            assignmentWatch = Stopwatch.StartNew();

            val1 = "4";
            val2 = 3;
            val3 = TimeSpan.FromSeconds(2);
            val4 = new Guid("00000000-0000-0000-0000-000000000001");

            writeCache[Tuple.Create(typeof(TestClass), "val1")].Item2(target, val1);
            writeCache[Tuple.Create(typeof(TestClass), "val2")].Item2(target, val2);
            writeCache[Tuple.Create(typeof(TestClass), "val3")].Item2(target, val3);
            writeCache[Tuple.Create(typeof(TestClass), "val4")].Item2(target, val4);

            assignmentWatch.Stop();
            var ms = assignmentWatch.ElapsedMilliseconds;
        };

        private It should_match_val1 = () => target.val1.ShouldEqual(val1);
        private It should_match_val2 = () => target.GetVal2().ShouldEqual(val2);
        private It should_match_val3 = () => target.GetVal3().ShouldEqual(val3);
        private It should_match_val4 = () => target.val4.ShouldEqual(val4);
    }
}
