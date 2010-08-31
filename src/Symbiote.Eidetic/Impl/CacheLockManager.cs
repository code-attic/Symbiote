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
            bool stored = Cache.Store(StoreMode.Add, FORMAT.AsFormat(lockId), check, TimeSpan.FromMinutes(1));
            return stored;
        }

        protected override void Release<T>(T lockId)
        {
            Cache.Remove(FORMAT.AsFormat(lockId));
        }

        public CacheLockManager(IRemember cache)
        {
            Cache = cache;
        }
    }
}
