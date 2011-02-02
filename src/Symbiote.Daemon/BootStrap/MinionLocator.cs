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
using Symbiote.Core.DI;

namespace Symbiote.Daemon.BootStrap
{
    public class MinionLocator : IMinionLocator
    {
        public Tuple<string, string, string> FindPrimaryAssembly(string fullPath)
        {
            var assemblies = GetAssembliesFromPath( fullPath ).ToList();
            Type minionType = null;
            var primary = assemblies
                .FirstOrDefault( x =>
                                     {
                                         try
                                         {
                                             var types = x.GetTypes();
                                             minionType =
                                                 types.FirstOrDefault(
                                                     t => t.GetInterface( "IMinion" ) != null );
                                         }
                                         catch ( Exception )
                                         {
                                         }
                                         return minionType != null;
                                     });
            return Tuple.Create( fullPath, primary.Location, minionType.FullName );
        }

        public IEnumerable<Assembly> GetAssembliesFromPath(string path)
        {
            //if (!Directory.Exists(path))
            //    return new Assembly[] { };

            return Directory.GetFiles(path)
                .Where(x => x.ToLower().EndsWith(".dll") || x.ToLower().EndsWith(".exe"))
                .Select(x =>
                {
                    Assembly assembly = null;
                    try
                    {
                        assembly = Assembly.ReflectionOnlyLoadFrom( x );
                    }
                    catch( Exception e )
                    {
                        var ex = e;
                    }
                    return assembly;
                })
                .Where(x => x != null);
        }
    }
}