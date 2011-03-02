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
using System.Linq.Expressions;

namespace Symbiote.JsonRpc.Host.Impl.Rpc
{
    public class RemoteFunc<T,R> : RemoteProcedure<T>
        where T : class
    {
        protected Expression<Func<T,R>> DeserializeTree()
        {
            var expressionParts = RebuildExpressionComponents();
            return
                Expression.Lambda<Func<T,R>>(
                    expressionParts.Item1,
                    expressionParts.Item2,
                    expressionParts.Item3
                );
        }

        public override object Invoke()
        {
            var instance = GetInstance();
            return DeserializeTree().Compile().Invoke(instance);
        }

        public RemoteFunc(string methodName, string json) : base(methodName, json)
        {
        }
    }
}