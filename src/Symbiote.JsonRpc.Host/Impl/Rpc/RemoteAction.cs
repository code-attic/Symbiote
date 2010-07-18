using System;
using System.Linq.Expressions;

namespace Symbiote.JsonRpc.Host.Impl.Rpc
{
    public class RemoteAction<T> : RemoteProcedure<T>
        where T : class
    {
        protected Expression<Action<T>> DeserializeTree()
        {
            var expressionParts = RebuildExpressionComponents();
            return
                Expression.Lambda<Action<T>>(
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

        public RemoteAction(string methodName, string json) : base(methodName, json)
        {
        }
    }
}