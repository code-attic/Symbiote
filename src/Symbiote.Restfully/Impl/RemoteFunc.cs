using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Symbiote.Restfully.Impl
{
    public class RemoteFunc<T,R> : RemoteProcedure<T>
        where T : class
    {
        protected Expression<Func<T,R>> DeserializeTree()
        {
            var expressionParts = RebuildExpressionComponents();
            return
                Expression.Lambda<Func<T,R>>(
                    expressionParts.Item1,
                    expressionParts.Item2,
                    expressionParts.Item3
                );
        }

        public override object Invoke()
        {
            var instance = GetInstance();
            DeserializeTree().Compile().Invoke(instance);
            return null;
        }

        public RemoteFunc(string methodName, string json) : base(methodName, json)
        {
        }
    }
}