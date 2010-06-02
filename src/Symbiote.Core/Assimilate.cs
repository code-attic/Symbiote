using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Config;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log;
using Symbiote.Core.Log.Impl;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace Symbiote.Core
{
    public interface IAssimilate
    {
    }

    public static class Assimilate 
    {
        private static IAssimilate _assimilate = new object() as IAssimilate;
        private static List<string> _assimilated = new List<string>();
        private static ReaderWriterLockSlim _assimilationLock = new ReaderWriterLockSlim();
        private static bool Initialized { get; set; }

        public static IAssimilate Core()
        {
            if (Initialized)
                return _assimilate;
            Initialized = true;

            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator());
            _assimilate.Dependencies(x =>
                         {
                             x.For<ILogProvider>().Use<NullLogProvider>();
                             x.For<ILogger>().Use<NullLogger>();
                             x.For(typeof (ILogger<>)).Add(typeof (ProxyLogger<>));
                             x.For<IJsonSerializerFactory>().Singleton().Use<JsonSerializerFactory>();
                             x.Scan(s =>
                                        {
                                            // oh the shameful hack :(
                                            // Machine.Specifications blows itself all to heck when
                                            // StructureMap scans it. I haven't really tried to determine why
                                            // but this will prevent the scan code from running
                                            if (Type.GetType("Machine.Specifications.Because, Machine.Specifications") == null)
                                            {
                                                s.AssembliesFromApplicationBaseDirectory();
                                                s.AddAllTypesOf<IContractResolverStrategy>();
                                            }
                                        });
                         });
            return _assimilate;
        }

        public static IAssimilate Register(this IAssimilate assimilate, Registry registry)
        {
            ObjectFactory.Configure(c => c.AddRegistry(registry));
            return assimilate;
        }

        public static IAssimilate Dependencies(this IAssimilate assimilate, Action<ConfigurationExpression> configuration)
        {
            ObjectFactory.Configure(configuration);
            return assimilate;
        }

        public static void Require(string prerequisite, Exception exception)
        {
            _assimilationLock.EnterReadLock();
            try
            {
                if(!_assimilated.Contains(prerequisite))
                {
                    throw exception;
                }
            }
            finally 
            {
                _assimilationLock.ExitReadLock();
            }
        }

        public static void RegisterSliver(string sliverName)
        {
            _assimilationLock.EnterWriteLock();
            try
            {
                if(!_assimilated.Contains(sliverName))
                    _assimilated.Add(sliverName);
            }
            finally
            {
                _assimilationLock.ExitWriteLock();
            }
        }
    }
}

