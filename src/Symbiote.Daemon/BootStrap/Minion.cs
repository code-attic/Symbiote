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
using System.Security.Policy;
using System.Threading;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Futures;

namespace Symbiote.Daemon.BootStrap
{
    public class Minion
    {
        public string Name { get; set; }
        public AppDomain DomainHandle { get; set; }
        public AppDomainSetup Setup { get; set; }
        public Evidence MinionEvidence { get; set; }
        public IInitialize Intializer { get; set; }
        public string DaemonDisplayName { get; set; }
        public string MinionPath { get; set; }
        public string CopyPath { get; set; }
        public bool Running { get; set; }
        public bool Starting { get; set; }
        public bool Stopping { get; set; }
        public object MinionLock { get; set; }

        public void StartUp()
        {
            lock(MinionLock)
            if(!Starting && !Running)
            {
                try
                {
                    "The Minion running at '{0}' has received a start command."
                        .ToInfo<IDaemon>(MinionPath);
                    Starting = true;
                    DomainHandle = AppDomain.CreateDomain(Setup.ApplicationName, AppDomain.CurrentDomain.Evidence, Setup);
                    var locator =
                        (MinionInitializer) DomainHandle
                        .CreateInstanceFromAndUnwrap( 
                            Path.Combine( CopyPath, "Symbiote.Daemon.dll" ),
                            typeof( MinionInitializer ).FullName,
                            false,
                            BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance,
                            null,
                            null,
                            null,
                            null
                        );
                        //.CreateInstanceFromAndUnwrap( "Symbiote.Daemon.dll", typeof( MinionInitializer ).FullName );
                    locator.InitializeMinion( MinionPath );
                    Running = true;
                    
                    // Create a cool-down period where the service cannot be
                    // reloaded due to files changing
                    Future.WithoutResult(() =>
                                             {
                                                 Thread.Sleep( TimeSpan.FromSeconds( 30 ) );
                                                 Starting = false;
                                             })
                          .Start();
                }
                catch ( AppDomainUnloadedException dying )
                {
                    "The Minion running at '{0}' has stopped running."
                        .ToInfo<IDaemon>( MinionPath );
                }
                catch ( Exception e )
                {
                    "An error occurred attempting to start the minion at '{0}'. \r\n\t {1}"
                        .ToError<IDaemon>(MinionPath, e);
                    Running = false;
                    Starting = false;
                }
            }
        }

        public void ShutItDown()
        {
            "The Minion running at '{0}' has received a shutdown command."
                        .ToInfo<IDaemon>(MinionPath);
            try
            {
                Stopping = true;
                Running = false;
                AppDomain.Unload( DomainHandle );
            }
            catch ( Exception e )
            {
                "An exception occurred while shutting down the minion at '{0}' \r\n\t"
                    .ToError<IDaemon>(MinionPath);
            }
            Stopping = false;
        }

        public Minion( string path )
        {
            MinionPath = Path.GetFullPath( path );
            var environment = Environment.CurrentDirectory;
            var applicationName = MinionPath.Split( Path.DirectorySeparatorChar ).Last();
            var shadowPath = Path.Combine( applicationName, "shadows" );
            CopyPath = Path.Combine( shadowPath, "references", applicationName );
            Directory.CreateDirectory( CopyPath );

            foreach (string newPath in Directory.GetFiles(MinionPath, "*.*", SearchOption.AllDirectories))
                File.Copy( newPath, newPath.Replace( MinionPath, CopyPath ), true );

            Setup = new AppDomainSetup();
            Setup.LoaderOptimization = LoaderOptimization.MultiDomainHost;
            Setup.ShadowCopyFiles = "true";
            Setup.ShadowCopyDirectories = MinionPath;
            Setup.CachePath = shadowPath;
            Setup.ApplicationName = applicationName;
            Setup.PrivateBinPath = CopyPath;
            //Setup.ApplicationBase = MinionPath;
            Setup.ApplicationBase = CopyPath;
            MinionEvidence = AppDomain.CurrentDomain.Evidence;
            MinionLock = new object();

            // It is important NOT to set the ApplicationBase property.
            // Setting it creates a system-level handle on the directory
            // effectively locking it so that nothing can delete/change
            // the directory itself.
        }
    }
}