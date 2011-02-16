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
        public ConcurrentBag<Type> CompleteTypeList { get; set; }

        public ConcurrentDictionary<Assembly, List<Type>> AssemblyTypes { get; set; }
        public ConcurrentDictionary<Type, Assembly> TypeAssemblies { get; set; }
        public ConcurrentDictionary<Type, List<Type>> ImplementorsOfType { get; set; }
        public ConcurrentDictionary<Type, List<Type>> TypeHierarchies { get; set; }
        public ConcurrentDictionary<Type, List<Type>> Closers { get; set; }

        public TypeScanner Scanner { get; set; }

        public void Start()
        {
            CompleteAssemblyList
                .AsParallel()
                .ForAll( LoadTypeList );
        }

        public void LoadTypeList( Assembly assembly )
        {
            var types = assembly.GetTypes().ToList();
            AssemblyTypes[assembly] = types;
            types.ForEach( t =>
                               {
                                   TypeAssemblies[t] = assembly;
                                   var parents = Reflector.GetInheritanceChain( t ).ToList();
                                   TypeHierarchies[t] = parents;
                                   parents.ForEach( p =>
                                                        {
                                                            ImplementorsOfType.AddOrUpdate( p,
                                                                                            c =>
                                                                                            new List<Type>( new[] {c} ),
                                                                                            ( c, l ) =>
                                                                                                {
                                                                                                    l.Add( c );
                                                                                                    return l;
                                                                                                } );
                                                            if ( p.IsOpenGeneric() && t.Closes( p ) )
                                                                Closers.AddOrUpdate( p,
                                                                                     c => new List<Type>( new[] {c} ),
                                                                                     ( c, l ) =>
                                                                                         {
                                                                                             l.Add( c );
                                                                                             return l;
                                                                                         } );
                                                        } );
                               } );

        }


        public ScanIndex()
        {
            Scanner = new TypeScanner();

            CompleteAssemblyList = new ConcurrentBag<Assembly>( Scanner.GetAssembliesFromBaseDirectory().ToList() );
            CompleteAssemblyList.ForEach( x => 
                { 

                } ); 
            CompleteTypeList = new ConcurrentBag<Type>(  );

            AssemblyTypes = new ConcurrentDictionary<Assembly, List<Type>>();
            TypeAssemblies = new ConcurrentDictionary<Type, Assembly>();
            ImplementorsOfType = new ConcurrentDictionary<Type, List<Type>>();
            TypeHierarchies = new ConcurrentDictionary<Type, List<Type>>();
            Closers = new ConcurrentDictionary<Type, List<Type>>();
        }
    }
}
