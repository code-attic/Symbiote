/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace Symbiote.JsonRpc.Host.Impl.Rpc
{
    public abstract class RemoteProcedure<T> : IRemoteProcedure
        where T : class
    {
        protected string Method { get; set; }
        protected string Json { get; set; }

        public abstract object Invoke();

        protected T GetInstance()
        {
            return Assimilate.GetInstanceOf<T>();
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