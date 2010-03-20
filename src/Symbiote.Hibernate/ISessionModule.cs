using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Hibernate
{
    public interface ISessionModule : IDisposable
    {
        void BeginSession();
        void EndSession();
    }
}
