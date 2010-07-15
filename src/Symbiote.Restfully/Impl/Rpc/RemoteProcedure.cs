using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json.Linq;
using Symbiote.Core.Extensions;

namespace Symbiote.Restfully.Impl.Rpc
{
    public abstract class RemoteProcedure<T> : IRemoteProcedure
        where T : class
    {
        protected string Method { get; set; }
        protected string Json { get; set; }

        public abstract object Invoke();

        protected T GetInstance()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }
        protected Tuple<Expression, bool, List<ParameterExpression>> RebuildExpressionComponents()
        {
            var stopWatch = Stopwatch.StartNew();
            var jObject = Json.FromJson() as JToken;
            var methodInfo = typeof(T).GetMethod(Method);
            var typeParam = Expression.Parameter(typeof(T), "x");
            var parameters = new List<ParameterExpression>() { typeParam };
            var tailCall = false;

            var argumentTypes = methodInfo.GetParameters().ToDictionary(x => x.Name, x => x.ParameterType);
            List<ConstantExpression> arguments = new List<ConstantExpression>();
            if (argumentTypes.Count > 0)
                arguments = jObject["arguments"]
                    .Cast<JProperty>()
                    .Select(x => Expression.Constant(x.Value.ToString().FromJson(argumentTypes[x.Name]))).ToList();

            Expression body = Expression.Call(typeParam, methodInfo, arguments);
            stopWatch.Stop();
            return Tuple.Create(body, tailCall, parameters);
        }

        public RemoteProcedure(string methodName, string json)
        {
            Method = methodName;
            Json = json;
        }
    }
}