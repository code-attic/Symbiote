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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;

namespace Symbiote.Core.DI {
    
    public class ScanIndex 
    {
        public List<Assembly> CompleteAssemblyList { get; set; }
        public Dictionary<Assembly, List<Type>> AssemblyTypes { get; set; }
        public Dictionary<Type, Assembly> TypeAssemblies { get; set; }
        public Dictionary<Type, List<Type>> ImplementorsOfType { get; set; }
        public Dictionary<Type, List<Type>> TypeHierarchies { get; set; }
        public Dictionary<Type, List<Type>> Closers { get; set; }
        public Dictionary<Type, Type> SingleImplementations { get; set; }
        public Dictionary<Assembly, bool> ConfiguredSymbiotes { get; set; }
        public Dictionary<Assembly, List<Assembly>> ReferenceLookup { get; set; }
        public List<Type> DependencyDefinitions { get; set; }
        public List<Type> ScanningInstructions { get; set; }
        public List<Type> SymbioteInitializers { get; set; }

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
            var dependencies = ImplementorsOfType.TryGet( typeof( IDefineStandardDependencies ) );
            var scanners = ImplementorsOfType.TryGet( typeof( IDefineScanningInstructions ) );
            var initializers = ImplementorsOfType.TryGet( typeof( IInitialize ) );

            DependencyDefinitions = dependencies.Item1
                ? dependencies.Item2.Where( x => x.IsConcrete() ).ToList()
                : new List<Type>();

            ScanningInstructions = scanners.Item1
                ? scanners.Item2.Where( x => x.IsConcrete() ).ToList()
                : new List<Type>();
            
            SymbioteInitializers = initializers.Item1
                ? initializers.Item2.Where( x => x.IsConcrete() ).ToList()
                : new List<Type>();

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
                                                                var l = new List<Type>(1000);
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
                                                                var l = new List<Type>(1000);
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

            var concreteTypesWithSingleImplementors = ImplementorsOfType
                .Where( x => !x.Key.IsConcrete() && x.Value.Count == 1 ).ToList();

            var assignable = concreteTypesWithSingleImplementors
                .Where( x => x.Value.First().IsConcreteAndAssignableTo( x.Key ) ).ToList();

            assignable.ForEach( x => SingleImplementations[x.Key] = x.Value.First() );
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
                    "WindowsBase",
                    "RabbitMQ.Client",
                    "log4net",
                } );
        }
		
		public List<Assembly> GetReferenceList( Assembly assembly )
		{
			List<Assembly> assemblyReferences = new List<Assembly>();
			try
			{
				assemblyReferences = assembly
                        .GetReferencedAssemblies()
                        .Select( Assembly.Load )
                        .ToList();
            	ReferenceLookup[assembly] = assemblyReferences;	
			}
			catch( Exception ex )
			{
				Console.WriteLine( ex );
			}
		    return assemblyReferences;
		}
		
        public ScanIndex()
        {
            AssemblyTypes = new Dictionary<Assembly, List<Type>>();
            TypeAssemblies = new Dictionary<Type, Assembly>();
            ImplementorsOfType = new Dictionary<Type, List<Type>>();
            TypeHierarchies = new Dictionary<Type, List<Type>>();
            Closers = new Dictionary<Type, List<Type>>();
            AssemblyExclusionList = new List<string>();
            ReferenceLookup = new Dictionary<Assembly, List<Assembly>>();
            ConfiguredSymbiotes = new Dictionary<Assembly, bool>();
            SingleImplementations = new Dictionary<Type, Type>();
            Scanner = new TypeScanner();
            
            InitExclusions();

            var initialList = new List<Assembly>( Scanner.GetAssembliesFromBaseDirectory().ToList() );
            var references = initialList
                .SelectMany( GetReferenceList )
                .ToList();
            initialList.AddRange( references.Where( r => !initialList.Any( a => a.FullName.Equals( r.FullName ) ) ) );

            var uniqueList = initialList.Distinct();
            var filtered = uniqueList.Where( x => !AssemblyExclusionList.Any( e => x.FullName.StartsWith( e ) ) );
            CompleteAssemblyList = new List<Assembly>( filtered.ToList() );
        }
    }
}
