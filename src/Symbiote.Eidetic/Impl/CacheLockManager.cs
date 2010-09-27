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
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Utility;

namespace Symbiote.Eidetic.Impl
{
    public class CacheLockManager
        : SpinLockManager
    {
        protected IRemember Cache { get; set; }
        protected const string FORMAT = "{0}_lock_key";

        protected override bool Lock<T>(T lockId, Guid check)
        {
            bool stored = false;
            try
            {
                stored = Cache.Store(StoreMode.Add, FORMAT.AsFormat(lockId), check, TimeSpan.FromMinutes(1));
            }
            catch
            {
            }
            return stored;
        }

        protected override void Release<T>(T lockId)
        {
            try
            {
                Cache.Remove(FORMAT.AsFormat(lockId));
            }
            catch
            {
            }
        }

        public CacheLockManager(IRemember cache)
        {
            Cache = cache;
        }
    }
}
