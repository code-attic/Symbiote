using System;

namespace Symbiote.Core.DI
{
    public interface IScan
    {
        void Scan(Action<IScanInstruction> scanInstruction);
    }
}