using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Collections;

namespace Core.Tests.Collections
{
    public class with_mrudictionary
    {
        public static MruDictionary<int, int> nrulookup;

        private Establish context = () =>
        {
            nrulookup = new MruDictionary<int, int>(10);
        };
    }
}
