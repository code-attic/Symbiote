using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;

namespace Core.Tests.DI
{
    public interface AnInterfaceOf<T> : AnInterfaceOf
    {
        
    }

    public abstract class with_assembly_scanning
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Initialize()
                                                .Dependencies(x => x.Scan(s =>
                                                                              {
                                                                                  s.AddAllTypesOf<IAmAnInterface>();
                                                                                  s.AddSingleImplementations();
                                                                              }));
                                        };

    }
}
