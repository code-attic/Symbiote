// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public Dictionary<Assembly, bool> ConfiguredSymbiotes { get; set; }
        public Dictionary<Assembly, List<Assembly>> ReferenceLookup { get; set; }
        public List<Type> DependencyDefinitions { get; set; }
        public List<Type> ScanningInstructions { get; set; }

        public TypeScanner Scanner { get; set; }

        public List<string> AssemblyExclusionList { get; set; }

        public void Start()
        {
            CompleteAssemblyList
                .ForEach( LoadTypeList );

            PopulateSymbioteList();
        }

        public void PopulateSymbioteList()
        {
            DependencyDefinitions = ImplementorsOfType[typeof( IDefineStandardDependencies )];
            ScanningInstructions = ImplementorsOfType[typeof( IDefineScanningInstructions )];
            var containingAssemblies =
                DependencyDefinitions.Select( x => x.Assembly )
                    .Concat( ScanningInstructions.Select( x => x.Assembly ) )
                    .Distinct();
            ConfiguredSymbiotes = containingAssemblies.ToDictionary( x => x, x => false );
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
                                                var parent = p.IsGenericType
                                                                 ? p.GetGenericTypeDefinition()
                                                                 : p;
                                                ImplementorsOfType
                                                    .AddOrUpdate( parent,
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
                                                    parent.IsOpenGeneric()
                                                    && t.Closes( parent );
                                                if ( closes )
                                                    Closers.AddOrUpdate( parent,
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
                    "StructureMap",
                    "Newtonsoft",
                    "protobuf-net",
                    "ServiceStack",
                    "WindowsBase"
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
            ReferenceLookup = new Dictionary<Assembly, List<Assembly>>();
            ConfiguredSymbiotes = new Dictionary<Assembly, bool>();
            Scanner = new TypeScanner();
            
            InitExclusions();

            var initialList = new List<Assembly>( Scanner.GetAssembliesFromBaseDirectory().ToList() );
            var references = initialList
                .SelectMany( x =>
                    {
                        var assemblyReferences = x
                            .GetReferencedAssemblies()
                            .Select( Assembly.Load )
                            .ToList();
                        ReferenceLookup[x] = assemblyReferences;
                        return assemblyReferences;
                    } )
                .ToList();
            initialList.AddRange( references );

            var uniqueList = initialList.Distinct();
            var filtered = uniqueList.Where( x => !AssemblyExclusionList.Any( e => x.FullName.StartsWith( e ) ) );
            CompleteAssemblyList = new ConcurrentBag<Assembly>( filtered.ToList() );
        }
    }
}
