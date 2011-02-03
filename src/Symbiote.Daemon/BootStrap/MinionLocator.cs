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
using System.IO;
using System.Linq;
using System.Reflection;
using Symbiote.Core.Collections;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.BootStrap
{
    public class MinionLocator : IMinionLocator
    {
        public ExclusiveConcurrentDictionary<string, Assembly> LoadedAssemblies { get; set; }
        public ExclusiveConcurrentDictionary<string, string> FoundMinions { get; set; }

        public Tuple<string, string, string> GetMinionFromPath(string fullPath)
        {
            var match = AnalyzeAssembliesInPath( fullPath );
            if(!match.Item1)
            {
                throw new Exception("No assemblies containing a type implementing IMinion were found at path '{0}'".AsFormat( fullPath ));
            }
            return Tuple.Create(fullPath, Path.Combine(fullPath, Path.GetFileName(match.Item2)), match.Item3);
        }

        public Tuple<bool, string, string> AnalyzeAssembliesInPath(string path)
        {
            return Directory.GetFiles( path )
                .Where( x => x.ToLower().EndsWith( ".dll" ) || x.ToLower().EndsWith( ".exe" ) )
                .Select( x =>
                    {
                        var assemblyFileName = Path.GetFileName( x );
                        var fullPath = Path.GetFullPath( x );
                        var minion = "";
                        try
                        {
                            var assembly = LoadedAssemblies.ReadOrWrite( assemblyFileName,
                                                                    () => Assembly.LoadFile(fullPath));
                            if ( assembly != null )
                            {
                                minion = FoundMinions.ReadOrWrite( 
                                    assemblyFileName, 
                                    () => GetMinionTypeFullName( assembly ) );
                            }
                        }
                        catch ( Exception e )
                        {
                            var ex = e;
                        }
                        return Tuple.Create(!string.IsNullOrEmpty(minion), assemblyFileName, minion);
                    } )
                .FirstOrDefault(x => x.Item1);
        }

        public string GetMinionTypeFullName( Assembly assembly )
        {
            var types = assembly.GetTypes();
            var minion = types.FirstOrDefault( t => t.GetInterface( "IMinion" ) != null );
            return minion == null ? null : minion.FullName;
        }

        public MinionLocator()
        {
            LoadedAssemblies = new ExclusiveConcurrentDictionary<string, Assembly>();
            FoundMinions = new ExclusiveConcurrentDictionary<string, string>();
        }
    }
}