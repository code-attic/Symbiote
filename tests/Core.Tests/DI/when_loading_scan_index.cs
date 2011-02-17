using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.DI;
using Symbiote.Messaging;

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
            var closers = index.Closers.Where( x => x.Value.Count == 1 ).ToList();
            var singleInterfaces = index.SingleImplementations;
        };
        
        private It should_take_3_seconds = () => 
            watch.ElapsedMilliseconds.ShouldBeLessThan( 1001 );
    }

    public class when_test_closing_aclassof
    {
        static bool closes;
        static bool open;

        private Because of = () => 
        { 
            open = typeof( AClassOf<> ).IsOpenGeneric();
            closes = typeof( ClosedClass ).Closes( typeof( AClassOf<> ) );
        };

        private It should_open_close = () => open.ShouldBeTrue();
        private It should_close = () => closes.ShouldBeTrue();
    }

    public class when_test_closing_aninterfaceof
    {
        static bool closes;
        static bool open;

        private Because of = () => 
        { 
            open = typeof( AnInterfaceOf<> ).IsOpenGeneric();
            closes = typeof( ClosedClass ).Closes( typeof( AnInterfaceOf<> ) );
        };

        private It should_close = () => closes.ShouldBeTrue();
        private It should_open_close = () => open.ShouldBeTrue();
    }
}
