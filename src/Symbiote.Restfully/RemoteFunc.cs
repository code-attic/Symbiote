using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Symbiote.Core.Extensions;

namespace Symbiote.Restfully
{
    public class RemoteFunc<T,R> : RemoteProcedure<T>
        where T : class
    {
        public override object Invoke()
        {
            T instance = GetInstance();
            var methodInfo = typeof(T).GetMethod(Method);
            object[] parameters = GetParameters(methodInfo);
            var result = methodInfo.Invoke(instance, parameters);
            return (R)result;
        }

        public RemoteFunc()
        {
        }

        public RemoteFunc(Expression<Func<T, R>> call)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            GetCallMetadata(call.Body as MethodCallExpression);
            stopwatch.Stop();
            var nurp = "";
        }
    }
}