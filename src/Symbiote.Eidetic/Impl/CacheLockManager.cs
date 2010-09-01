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
