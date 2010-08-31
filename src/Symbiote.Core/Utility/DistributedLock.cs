using System;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Core.Utility
{
    public class DistributedLock
        : IDisposable
    {
        protected ILockManager Manager { get; set; }
        protected object LockId { get; set; }

        public static DistributedLock Create<T>(T lockId)
        {
            var manager = ServiceLocator.Current.GetInstance<ILockManager>();
            var lockInstance = new DistributedLock(manager, lockId);
            manager.AcquireLock(lockId);
            return lockInstance;
        }

        public DistributedLock(ILockManager lockManager, object lockId)
        {
            Manager = lockManager;
            LockId = lockId;
        }

        public void Dispose()
        {
            Manager.ReleaseLock(LockId);
        }
    }
}