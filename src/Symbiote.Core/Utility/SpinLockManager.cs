using System;
using System.Diagnostics;
using System.Threading;

namespace Symbiote.Core.Utility
{
    public abstract class SpinLockManager
        : ILockManager
    {
        public virtual bool AcquireLock<T>(T lockId)
        {
            var newId = Guid.NewGuid();
            var acquired = false;
            while(!acquired)
            {
                acquired = AttemptAcquisition(lockId, newId);
                if(!acquired)
                    Thread.Sleep(15);
            }
            return acquired;
        }

        protected virtual bool AttemptAcquisition<T>(T lockId, Guid value)
        {
            var acquired = Lock(lockId, value);
            return acquired;
        }

        public virtual void ReleaseLock<T>(T lockId)
        {
            Release(lockId);
        }

        protected abstract bool Lock<T>(T lockId, Guid check);
        protected abstract void Release<T>(T lockId);
    }
}