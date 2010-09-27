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
using System.ServiceModel.Description;

namespace Symbiote.Wcf.Client
{
    public class NullServiceClient<T> : IService<T>
        where T : class
    {
        public void Dispose()
        {

        }

        public ClientCredentials ClientCredentials
        {
            get { throw new NotImplementedException(); }
        }

        public void Call(Action<T> call)
        {
            throw new NotImplementedException();
        }

        public TResult Call<TResult>(Func<T, TResult> call)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}