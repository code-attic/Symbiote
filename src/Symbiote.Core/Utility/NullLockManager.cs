namespace Symbiote.Core.Utility
{
    public class NullLockManager
        : ILockManager
    {
        public bool AcquireLock<T>(T lockId)
        {
            throw new AssimilationException("No valid lock manager implementation has been provided. Distributed locks require a valid lock manager configuration. Please consider Eidetic, Hibernate or Relax for this functionality.");
        }

        public void ReleaseLock<T>(T lockId)
        {
            
        }
    }
}