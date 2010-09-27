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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Symbiote.Core.Extensions;
using Symbiote.JsonRpc.Client.Impl.Rpc;

namespace Symbiote.JsonRpc.Client.Impl
{
    public static class RPCExtensions
    {
        public static string GetJsonForArguments(this MethodCallExpression methodCallExpression)
        {
            var stopWatch = Stopwatch.StartNew();
            var argNames = methodCallExpression.Method.GetParameters().Select(x => x.Name);
            var argValues = methodCallExpression.Arguments.Select(x => Expression.Lambda(x).Compile().DynamicInvoke());
            var arguments = argNames.ZipToDictionary(argValues);

            using (var textWriter = new StringWriter())
            using (var writer = new JsonTextWriter(textWriter))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("arguments");
                writer.WriteStartObject();
                arguments.ForEach(x =>
                {
                    writer.WritePropertyName(x.Key);
                    writer.WriteRawValue(x.Value.ToJson(false));
                });
                writer.WriteEndObject();
                writer.WriteEndObject();
                writer.Flush();
                stopWatch.Stop();
                return textWriter.ToString();
            }
        }
    }
}
