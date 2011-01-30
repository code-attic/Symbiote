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
using System.Configuration.Install;
using System.Reflection;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.Installation
{
    public class UninstallCommand
        : IDaemonCommand
    {
        private readonly string ASSEMBLY_PATH = "/assemblypath={0}";
        private readonly Assembly ENTRY_ASSEMBLY = Assembly.GetEntryAssembly();

        public DaemonInstaller Installer { get; set; }
        public ICheckPermission PermissionCheck { get; set; }

        public string[] CommandLine
        {
            get { return new[] {ASSEMBLY_PATH.AsFormat( ENTRY_ASSEMBLY.Location )}; }
        }

        #region IDaemonCommand Members

        public void Execute()
        {
            if ( Installer.Installed )
            {
                if ( PermissionCheck.HasPermission() )
                {
                    Unregister();
                }
            }
        }

        #endregion

        protected void Unregister()
        {
            using( var transactedInstall = new TransactedInstaller() )
            {
                transactedInstall.Installers.Add( Installer );
                if ( ENTRY_ASSEMBLY != null )
                {
                    transactedInstall.Context = new InstallContext( null, CommandLine );
                    transactedInstall.Uninstall( null );
                }
            }
        }

        public UninstallCommand( DaemonInstaller installer, ICheckPermission permissionCheck )
        {
            Installer = installer;
            PermissionCheck = permissionCheck;
        }
    }
}