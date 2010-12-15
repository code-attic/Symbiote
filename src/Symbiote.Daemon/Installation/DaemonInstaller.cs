/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections;
using System.Configuration.Install;

namespace Symbiote.Daemon.Installation
{
    public class DaemonInstaller
        : Installer
    {
        protected DaemonConfiguration Configuration { get; set; }
        public bool Installed { get { return Configuration.Installed(); } }

        public override void Install( IDictionary stateSaver )
        {
            InitializeInstallers();
            base.Install( stateSaver );
            Configuration.UpdateRegistry();
        }

        public override void Uninstall( IDictionary savedState )
        {
            InitializeInstallers();
            base.Uninstall( savedState );
        }

        public void InitializeInstallers()
        {
            Installers.AddRange(new Installer[]
            {
                Configuration.GetServiceInstaller(),
                Configuration.GetProcessInstaller()
            });
        }

        public DaemonInstaller( DaemonConfiguration configuration )
        {
            Configuration = configuration;
        }
    }
}
