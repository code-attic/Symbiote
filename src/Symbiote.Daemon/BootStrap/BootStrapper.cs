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
using System.Linq;
using Symbiote.Core.Extensions;
using Symbiote.Daemon.BootStrap.Config;
using Symbiote.Messaging;

namespace Symbiote.Daemon.BootStrap
{
    public class BootStrapper : IBootStrapper
    {
        public BootStrapConfiguration Configuration { get; set; }
        public IBus Bus { get; set; }
        public Watcher MinionWatcher { get; set; }

        public void Start()
        {
            RunMinions();
            MinionWatcher.Start();
        }

        public void RunMinions()
        {
            var paths = Configuration.WatchPaths.SelectMany( Directory.GetDirectories );
            paths.ForEach( x => Bus.Publish( "local", new NewApplication() {DirectoryPath = Path.GetFullPath(x)} ) );
        }

        public BootStrapper( DaemonConfiguration configuration, IBus bus, Watcher minionWatcher )
        {
            Configuration = configuration.BootStrapConfiguration;
            Bus = bus;
            MinionWatcher = minionWatcher;
            bus.AddLocalChannel( x => x
                                .CorrelateBy<NewApplication>( m => m.DirectoryPath )
                                .CorrelateBy<ApplicationChanged>( m => m.DirectoryPath )
                                .CorrelateBy<ApplicationDeleted>( m => m.DirectoryPath ) );
        }
    }
}