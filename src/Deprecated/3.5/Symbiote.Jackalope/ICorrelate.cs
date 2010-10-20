using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Jackalope
{
    public interface ICorrelate
    {
        string CorrelationId { get; set; }
    }
}
