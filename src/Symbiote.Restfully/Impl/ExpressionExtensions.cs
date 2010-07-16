using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Symbiote.Core.Extensions;
using Symbiote.JsonRpc.Impl.Rpc;

namespace Symbiote.JsonRpc.Impl
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

        public static object InvokeRemoteProcedure(this MethodInfo methodInfo, Type contractType, string jsonArgs)
        {
            var stopWatch = Stopwatch.StartNew();
            var returnType = methodInfo.ReturnType;
            Type procType = null;
            if(!returnType.Name.Contains("Void"))
            {
                procType = typeof (RemoteFunc<,>).MakeGenericType(contractType, returnType);
            }
            else
            {
                procType = typeof(RemoteAction<>).MakeGenericType(contractType);
            }

            var procedure = Activator.CreateInstance(procType, methodInfo.Name, jsonArgs) as IRemoteProcedure;
            var result = procedure.Invoke();
            stopWatch.Stop();
            return result;
        }
    }
}
