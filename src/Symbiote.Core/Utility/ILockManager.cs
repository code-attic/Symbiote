using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Utility
{
    public interface ILockManager
    {
        bool AcquireLock<T>(T lockId);
        void ReleaseLock<T>(T lockId);
    }
}
