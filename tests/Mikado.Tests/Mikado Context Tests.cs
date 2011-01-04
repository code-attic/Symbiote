using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Mikado.Tests.Domain.Model;
using Mikado.Tests.TestSetup;
using Symbiote.Core.Memento;
using Symbiote.Core.Work;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests
{
    public class when_testing_broken_rules_on_a_Person_in_a_Mikado_Context : with_Person
    {
        //private Because of = () =>
        //                         {
        //                             var provider = new MikadoContextProvider(new EventConfiguration(), new Memoizer(), new EventPublisher(),  )
        //                         };
    }
}
