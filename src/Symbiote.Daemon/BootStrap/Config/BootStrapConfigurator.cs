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
using System.IO;
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.BootStrap.Config
{
    public class BootStrapConfigurator
    {
        public BootStrapConfiguration Configuration { get; set; }

        public BootStrapConfigurator HostApplicationsFrom(string path)
        {
            var fullPath = Path.GetFullPath( path );
            if( !Directory.Exists( fullPath ) )
            {
                throw new AssimilationException( 
                    "The path '{0}' does not exist, Daemon cannot host applications from non-existent directories or directories its process does not have permissions to."
                        .AsFormat( fullPath ) );
            }
            Configuration.WatchPaths.Add( fullPath );
            return this;
        }

        public BootStrapConfigurator WatchFilesLike(string pattern)
        {
            Configuration.FileExtensions.Add( pattern );
            return this;
        }

        public BootStrapConfigurator()
        {
            Configuration = new BootStrapConfiguration();
        }
    }
}