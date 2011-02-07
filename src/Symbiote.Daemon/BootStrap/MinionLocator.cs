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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.BootStrap
{
    [Serializable]
    public class MinionLocator : MarshalByRefObject
    {
        public object GetMinionHost(string fullPath)
        {
            var match = AnalyzeAssembliesInPath( fullPath );
            if(!match.Item1)
            {
                throw new Exception("No assemblies containing a type implementing IMinion were found at path '{0}'".AsFormat( fullPath ));
            }
            var minionHost = Activator.CreateInstance( match.Item3 );
            return minionHost;
        }

        public Tuple<bool, Assembly, Type> AnalyzeAssembliesInPath(string path)
        {
            var tries = 0;
            while(!Directory.Exists( path ) && tries < 10)
            {
                Thread.Sleep( 1000 );
                tries++;
            }
            var files = Directory.GetFiles( path );
            return files
                .Where( x => ( x.ToLower().EndsWith( ".dll" ) || x.ToLower().EndsWith( ".exe" ) ) )
                .Select( x =>
                    {
                        var fullPath = Path.GetFullPath( x );
                        return AppDomain.CurrentDomain.Load( AssemblyName.GetAssemblyName( x ) );
                    })
                .ToList()
                .Select( x =>
                    {
                        Type minion = null;
                        try
                        {
                            minion = GetMinion(x);
                        }
                        catch (Exception e)
                        {
                            var ex = e;
                        }
                        return Tuple.Create(minion != null, x, minion);
                    })
                .FirstOrDefault(x => x.Item1);
        }

        public Type GetMinion( Assembly assembly )
        {
            var types = assembly.GetTypes();
            var minion = types.FirstOrDefault( t => t.GetInterface( "IMinion" ) != null );
            return minion;
        }

        public MinionLocator()
        {
        }
    }
}