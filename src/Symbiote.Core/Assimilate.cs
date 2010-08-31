using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.DI;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log;
using Symbiote.Core.Log.Impl;
using Symbiote.Core.Utility;

namespace Symbiote.Core
{
    public interface IAssimilate
    {
        IDependencyAdapter DependencyAdapter { get; set; }
    }

    public class Assimilation : IAssimilate
    {
        public IDependencyAdapter DependencyAdapter { get; set; }

        public Assimilation()
        {

        }
    }

    public static class Assimilate 
    {
        public static IAssimilate Assimilation { get; set; }
        private static List<string> _assimilated = new List<string>();
        
#if !SILVERLIGHT
        private static ReaderWriterLockSlim _assimilationLock = new ReaderWriterLockSlim();
#endif

#if SILVERLIGHT
        private static object _assimilationLock = new object();
#endif

        private static bool Initialized { get; set; }

        static Assimilate()
        {
            Assimilation = new Assimilation();    
        }

        public static IAssimilate Core<TDepedencyAdapter>()
            where TDepedencyAdapter : class, IDependencyAdapter, new()
        {
            var adapter = Activator.CreateInstance<TDepedencyAdapter>();
            return Core(adapter);
        }

        public static IAssimilate Core(IDependencyAdapter adapter)
        {
            if (Initialized)
                return Assimilation;
            Initialized = true;

            Assimilation.DependencyAdapter = adapter;

            ServiceLocator.SetLocatorProvider(() => Assimilation.DependencyAdapter);

            Assimilation.Dependencies(x =>
                         {
                             x.For<ILogProvider>().Use<NullLogProvider>();
                             x.For<ILogger>().Use<NullLogger>();
                             x.For(typeof (ILogger<>)).Add(typeof (ProxyLogger<>));
                             x.For<ILockManager>().Use<NullLockManager>();
                             x.For<IJsonSerializerFactory>().Use<JsonSerializerFactory>().AsSingleton();
                             x.Scan(s =>
                                        {
                                            
                                            // oh the shameful hack :(
                                            // Machine.Specifications blows itself all to heck when
                                            // StructureMap scans it. I haven't really tried to determine why
                                            // but this will prevent the scan code from running
                                            s.AssembliesFromApplicationBaseDirectory(a =>
                                                {
                                                    var fullName = a.FullName;
                                                    return !
                                                        (fullName.Contains("Microsoft") ||
                                                            fullName.Contains("Machine.Specifications") ||
                                                        fullName.Contains("Gallio")
                                                        );
                                                });
                                            s.AddAllTypesOf<IContractResolverStrategy>();
                                        });
                             x.For<IDependencyAdapter>().Use(adapter);
                         });
            return Assimilation;
        }

        public static IAssimilate Dependencies(this IAssimilate assimilate, Action<DependencyConfigurator> configurator)
        {
            var config = new DependencyConfigurator();
            configurator(config);
            config.RegisterDependencies(assimilate.DependencyAdapter);
            return assimilate;
        }

        public static IAssimilate Dependencies(Action<DependencyConfigurator> configurator)
        {
            var config = new DependencyConfigurator();
            configurator(config);
            config.RegisterDependencies(Assimilation.DependencyAdapter);
            return Assimilation;
        }

        public static void Require(string prerequisite, Exception exception)
        {
#if !SILVERLIGHT
            _assimilationLock.EnterReadLock();
#endif
#if SILVERLIGHT
        lock(_assimilationLock)
        {
#endif
            try
            {
                if(!_assimilated.Contains(prerequisite))
                {
                    throw exception;
                }
            }
            finally 
            {
#if !SILVERLIGHT
                _assimilationLock.ExitReadLock();
#endif
            }
#if SILVERLIGHT
        }
#endif
        }

        public static void RegisterSymbioteLibrary(string sliverName)
        {
#if !SILVERLIGHT
            _assimilationLock.EnterWriteLock();
#endif
#if SILVERLIGHT
        lock(_assimilationLock)
        {
#endif
            try
            {
                if(!_assimilated.Contains(sliverName))
                    _assimilated.Add(sliverName);
            }
            finally
            {
#if !SILVERLIGHT
                _assimilationLock.ExitWriteLock();
#endif
            }
#if SILVERLIGHT
        }
#endif
        }
    }
}

