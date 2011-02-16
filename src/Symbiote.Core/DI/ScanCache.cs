using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;

namespace Symbiote.Core.DI {
    
    public class ScanIndex 
    {
        public ConcurrentBag<Assembly> CompleteAssemblyList { get; set; }
        public ConcurrentDictionary<Assembly, List<Type>> AssemblyTypes { get; set; }
        public ConcurrentDictionary<Type, Assembly> TypeAssemblies { get; set; }
        public ConcurrentDictionary<Type, List<Type>> ImplementorsOfType { get; set; }
        public ConcurrentDictionary<Type, List<Type>> TypeHierarchies { get; set; }
        public ConcurrentDictionary<Type, List<Type>> Closers { get; set; }
        public ConcurrentDictionary<Type, Type> SingleImplementations { get; set; }

        public TypeScanner Scanner { get; set; }

        public List<string> AssemblyExclusionList { get; set; }

        public void Start()
        {
            CompleteAssemblyList
                .AsParallel()
                .ForAll( LoadTypeList );
        }

        public void LoadTypeList( Assembly assembly )
        {
            var types = assembly.GetTypes().ToList();
            var objectType = typeof( Object );
            AssemblyTypes[assembly] = types;
            types.ForEach( t =>
                               {
                                   TypeAssemblies[t] = assembly;
                                   var parents = Reflector.GetInheritanceChain( t ).Where( o => !o.Equals( objectType ) ).ToList();
                                   TypeHierarchies[t] = parents;
                                   parents.ForEach( p =>
                                            {
                                                ImplementorsOfType
                                                    .AddOrUpdate( p,
                                                        c =>
                                                            { 
                                                                var l = new List<Type>(6000);
                                                                l.Add( t );
                                                                return l;
                                                            },
                                                        ( c, l ) =>
                                                            {
                                                                l.Add( t );
                                                                return l;
                                                            } );
                                                var closes = 
                                                    p.IsOpenGeneric()
                                                    && t.Closes( p );
                                                if ( closes )
                                                    Closers.AddOrUpdate( p,
                                                        c => 
                                                            { 
                                                                var l = new List<Type>(6000);
                                                                l.Add( t );
                                                                return l;
                                                            },
                                                        ( c, l ) =>
                                                            {
                                                                l.Add( t );
                                                                return l;
                                                            } );
                                            } );
                               } );

            SingleImplementations = new ConcurrentDictionary<Type, Type>(
                ImplementorsOfType
                    .Where( x => x.Value.Count == 1 )
                    .Select( x => new KeyValuePair<Type, Type>( x.Key, x.Value.First() ) ) );

            
        }

        public void InitExclusions()
        {
            AssemblyExclusionList
                .AddRange( new string[] 
                {
                    "System",
                    "Microsoft",
                    "mscorlib",
                    "FSharp.Core",
                    "Moq",
                    "Machine.Specifications",
                    "StructureMap"
                } );
        }

        public ScanIndex()
        {
            AssemblyTypes = new ConcurrentDictionary<Assembly, List<Type>>();
            TypeAssemblies = new ConcurrentDictionary<Type, Assembly>();
            ImplementorsOfType = new ConcurrentDictionary<Type, List<Type>>();
            TypeHierarchies = new ConcurrentDictionary<Type, List<Type>>();
            Closers = new ConcurrentDictionary<Type, List<Type>>();
            AssemblyExclusionList = new List<string>();
            Scanner = new TypeScanner();
            
            InitExclusions();

            var initialList = new List<Assembly>( Scanner.GetAssembliesFromBaseDirectory().ToList() );
            var references = initialList
                .SelectMany( x => x.GetReferencedAssemblies().Select( Assembly.Load ) )
                .ToList();
            initialList.AddRange( references );

            var uniqueList = initialList.Distinct();
            var filtered = uniqueList.Where( x => !AssemblyExclusionList.Any( e => x.FullName.StartsWith( e ) ) );
            CompleteAssemblyList = new ConcurrentBag<Assembly>( filtered.ToList() );
            
        }
    }
}
