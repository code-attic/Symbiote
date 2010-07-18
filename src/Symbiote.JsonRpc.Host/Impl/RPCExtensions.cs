using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Symbiote.Core.Extensions;
using Symbiote.JsonRpc.Host.Impl.Rpc;

namespace Symbiote.JsonRpc.Host.Impl
{
    public static class RPCExtensions
    {
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
