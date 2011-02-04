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
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.BootStrap
{
    public class Minion
    {
        public AppDomain DomainHandle { get; set; }
        public AppDomainSetup Setup { get; set; }
        public Evidence MinionEvidence { get; set; }
        public IMinion Instance { get; set; }
        public string DaemonDisplayName { get; set; }
        public string MinionPath { get; set; }
        public bool Running { get; set; }
        public bool Starting { get; set; }
        public bool Stopping { get; set; }

        public void StartUp()
        {
            if(!Starting && !Running)
            {
                DomainHandle = AppDomain.CreateDomain(Setup.ApplicationName, AppDomain.CurrentDomain.Evidence, Setup);
                //var locator = (MinionLocator) daemon.CreateInstance(typeof(MinionLocator).FullName);
                var daemon = DomainHandle.Load( DaemonDisplayName );
                var locator =
                    (MinionLocator)
                    DomainHandle.CreateInstanceFromAndUnwrap( "Symbiote.Daemon.dll", typeof( MinionLocator ).FullName );
                var host = (IMinion) locator.GetMinionHost(MinionPath);
                host.Start( null );
            }
        }

        public void ShutItDown()
        {
            AppDomain.Unload( DomainHandle );
        }

        public Minion( string path )
        {
            MinionPath = Path.GetFullPath( path );
            Setup = new AppDomainSetup();
            //Setup.LoaderOptimization = LoaderOptimization.MultiDomain;
            Setup.ShadowCopyFiles = "true";
            Setup.ShadowCopyDirectories = MinionPath;
            Setup.CachePath = @"c:\shadows";
            Setup.ApplicationName = MinionPath.Split( Path.DirectorySeparatorChar ).Last();
            Setup.ApplicationBase = MinionPath;
            Setup.PrivateBinPath = MinionPath;
            //MinionEvidence = AppDomain.CurrentDomain.Evidence;
            DaemonDisplayName = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .First(x => x.FullName.Contains("Symbiote.Daemon"))
                .FullName;
        }
    }
}