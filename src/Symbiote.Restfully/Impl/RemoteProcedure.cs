using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using StructureMap;
using Symbiote.Core.Extensions;

namespace Symbiote.Restfully.Impl
{
    public abstract class RemoteProcedure<T> : IRemoteProcedure
        where T : class
    {
        public string Contract { get { return typeof (T).Name; }}
        public string Method { get; set; }

        public abstract object Invoke();
        public abstract string JsonExpressionTree { set; }

        protected T GetInstance()
        {
            return ObjectFactory.GetInstance<T>();
        }

        protected Tuple<Expression, bool, List<ParameterExpression>> RebuildExpressionComponents(JToken jObject)
        {
            var methodInfo = typeof(T).GetMethod(Method);
            var typeParam = Expression.Parameter(typeof(T), jObject["Body"]["Object"]["Name"].Value<string>());
            var parameters = new List<ParameterExpression>() { typeParam };
            var tailCall = jObject["TailCall"].Value<bool>();
            var arguments = jObject["Body"]["Arguments"]["$values"].Select(x =>
            {
                var type = Type.GetType(x["Type"].Value<string>());
                var val = x["Value"].ToString().FromJson(type);
                return Expression.Constant(val, type);
            }).ToList();

            Expression body = Expression.Call(typeParam, methodInfo, arguments);

            return Tuple.Create(body, tailCall, parameters);
        }

        protected void GetCallMetadata(MethodCallExpression methodCallExpression)
        {
            Method = methodCallExpression.Method.Name;
        }

        public RemoteProcedure()
        {
        }
    }
}