using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Symbiote.Restfully.Impl
{
    public class RemoteFunc<T,R> : RemoteProcedure<T>
        where T : class
    {
        public Expression<Func<T,R>> ExpressionTree { get; protected set; }

        [JsonIgnore]
        public override string JsonExpressionTree
        {
            set
            {
                var jObject = JObject.Parse(value);
                ExpressionTree = DeserializeTree(jObject["ExpressionTree"]);
            }
        }

        protected Expression<Func<T, R>> DeserializeTree(JToken jObject)
        {
            var expressionParts = RebuildExpressionComponents(jObject);
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
            return ExpressionTree.Compile().Invoke(instance);
        }

        public RemoteFunc()
        {
        }

        public RemoteFunc(Expression<Func<T, R>> call)
        {
            ExpressionTree = call.ChangeArgsToConstants();
            GetCallMetadata(call.Body as MethodCallExpression);
        }
    }
}