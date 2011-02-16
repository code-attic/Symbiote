using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.DI;

namespace Core.Tests.DI {
    
    public class when_loading_scan_index 
    {
        static Stopwatch watch;
        static ScanIndex index;

        private Because of = () => 
        {
            watch = Stopwatch.StartNew();

            index = new ScanIndex();
            index.Start();

            watch.Stop();

            var assemblyCount = index.CompleteAssemblyList.Count;
            var typeCount = index.CompleteTypeList.Count;
            var closers = index.Closers.Where( x => x.Value.Count == 1 ).ToList();
            var singleInterfaces = index.ImplementorsOfType.Where( x => x.Value.Count == 1 ).ToList();

            int f = 0;
        };
        
        private It should_take_2_seconds = () => 
            watch.ElapsedMilliseconds.ShouldBeLessThan( 2001 );
    }
}
