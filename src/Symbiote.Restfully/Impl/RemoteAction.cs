using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Symbiote.Restfully.Impl
{
    public class RemoteAction<T> : RemoteProcedure<T>
        where T : class
    {
        public Expression<Action<T>> ExpressionTree { get; protected set; }
        
        [JsonIgnore]
        public override string JsonExpressionTree
        {
            set
            {
                var jObject = JObject.Parse(value);
                ExpressionTree = DeserializeTree(jObject["ExpressionTree"]);
            }
        }

        protected Expression<Action<T>> DeserializeTree(JToken jObject)
        {
            var expressionParts = RebuildExpressionComponents(jObject);
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
            ExpressionTree.Compile().Invoke(instance);
            return null;
        }

        public RemoteAction()
        {
        }

        public RemoteAction(Expression<Action<T>> call)
        {
            ExpressionTree = call.ChangeArgsToConstants();
            GetCallMetadata(call.Body as MethodCallExpression);
        }
    }
}