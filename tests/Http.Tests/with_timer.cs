using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Symbiote.Http.Owin;

namespace Http.Tests
{
    public abstract class with_timer
    {
        public static Stopwatch Timer { get; set; }
    }
}
