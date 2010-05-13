using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;
using StructureMap;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;

namespace Symbiote.Restfully
{
    public interface IRemoteProcedure
    {
        string Contract { get; }
        string Method { get; }
        Dictionary<string, string> Args { get; set; }
        object Invoke();
        JObject JsonExpressionTree { set; }
    }

    public abstract class RemoteProcedure<T> : IRemoteProcedure
        where T : class
    {
        public string Contract { get { return typeof (T).Name; }}
        public string Method { get; set; }
        public Dictionary<string, string> Args { get; set; }

        public abstract object Invoke();

        protected T GetInstance()
        {
            return ObjectFactory.GetInstance<T>();
        }

        protected void GetCallMetadata(MethodCallExpression methodCallExpression)
        {
            Method = methodCallExpression.Method.Name;

            var arguments = methodCallExpression.Arguments;

            var values = arguments
                .Select(x => GetExpressionValue(x))
                .ToArray();

            int valueIndex = 0;
            Args = methodCallExpression
                .Method
                .GetParameters()
                .ToDictionary<ParameterInfo, string, string>(x => x.Name, x => values[valueIndex++]);
        }
        
        protected string GetExpressionValue(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke().ToJson();
        }

        protected object[] GetParameters(MethodInfo methodInfo)
        {
            var index = 0;
            var source = Args.Values.Select(x => x.ToString());
            var types = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            return source.Select(x => x.FromJson(types[index++])).ToArray();
        }

        public RemoteProcedure()
        {
            Args = new Dictionary<string, string>();
        }
    }
}