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

namespace Symbiote.Daemon.BootStrap
{
    public class Minion
    {
        public AppDomain DomainHandle { get; set; }
        public AppDomainManager DomainManager { get; set; }
        public AppDomainSetup Setup { get; set; }
        public MinionLocator Locator { get; set; }
        public Evidence MinionEvidence { get; set; }
        public string MinionPath { get; set; }
        public string MinionType { get; set; }
        public string PrimaryAssembly { get; set; }
        public bool Running { get; set; }
        public bool Starting { get; set; }
        public bool Stopping { get; set; }

        public void StartUp()
        {
            if(!Starting && !Running)
            {
                DomainHandle = AppDomain.CreateDomain(MinionPath, null, Setup);
                var host =
                    (IMinion)
                    DomainHandle.CreateInstanceFromAndUnwrap( Path.Combine( MinionPath, PrimaryAssembly ),
                                                              MinionType, false, 0, null, null, null, null );
                host.Start( null );
            }
        }

        public void ShutItDown()
        {
            AppDomain.Unload( DomainHandle );
        }

        public Minion( string path )
        {
            Locator = new MinionLocator();
            DomainManager = new AppDomainManager();

            var minion = Locator.GetMinionFromPath( path );
            MinionPath = minion.Item1;
            PrimaryAssembly = minion.Item2;
            MinionType = minion.Item3;

            Setup = new AppDomainSetup();
            Setup.LoaderOptimization = LoaderOptimization.MultiDomain;
            Setup.ShadowCopyFiles = "true";
            Setup.ShadowCopyDirectories = MinionPath;
            Setup.CachePath = @"/shadows";
            Setup.ApplicationName = MinionPath.Split( Path.DirectorySeparatorChar ).Last();
            Setup.ApplicationBase = MinionPath;
            Setup.PrivateBinPath = MinionPath;
            MinionEvidence = AppDomain.CurrentDomain.Evidence;
        }
    }
}