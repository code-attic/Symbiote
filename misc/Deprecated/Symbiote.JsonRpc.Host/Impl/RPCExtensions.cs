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
