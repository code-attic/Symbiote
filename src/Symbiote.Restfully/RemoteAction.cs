using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Symbiote.Restfully
{
    public class RemoteAction<T> : RemoteProcedure<T>
        where T : class
    {
        public Expression<Action<T>> ExpressionTree { get; protected set; }
        
        [JsonIgnore]
        public JObject JsonExpressionTree
        {
            set
            {
                ExpressionTree = DeserializeTree(value);             
            }
        }

        protected Expression<Action<T>> DeserializeTree(JObject jObject)
        {
            var methodInfo = typeof (T).GetMethod(Method);
            var typeParam = Expression.Parameter(typeof (T), jObject["Body"]["Object"]["Name"].Value<string>());
            var arguments = jObject["Body"]["Arguments"].Values().Select(x => Expression.Constant(x.CreateReader().Value));
            Expression body = Expression.Call(typeParam, methodInfo, arguments);
            var parameters = new List<ParameterExpression>() { typeParam };
            return
                Expression.Lambda<Action<T>>(
                    body,
                    jObject["TailCall"].Value<bool>(),
                    parameters
                );
        }

        public override object Invoke()
        {
            T instance = GetInstance();
            var methodInfo = typeof(T).GetMethod(Method);
            object[] parameters = GetParameters(methodInfo);
            methodInfo.Invoke(instance, parameters);
            return null;
        }

        public RemoteAction()
        {
        }

        public RemoteAction(Expression<Action<T>> call)
        {
            ExpressionTree = call;
            GetCallMetadata(call.Body as MethodCallExpression);
        }
    }
}