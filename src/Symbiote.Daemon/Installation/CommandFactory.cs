﻿/* 
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

using Symbiote.Core;

namespace Symbiote.Daemon.Installation
{
    public class CommandFactory
    {
        public DaemonConfiguration Configuration { get; set; }

        public IDaemonCommand GetCommand()
        {
            IDaemonCommand command;
            if (Configuration.Arguments.Install)
            {
                command = Assimilate.GetInstanceOf<InstallCommand>();
            }
            else if (Configuration.Arguments.Uninstall)
            {
                command = Assimilate.GetInstanceOf<UninstallCommand>();
            }
            else
            {
                command = Assimilate.GetInstanceOf<RunCommand>();
            }
            return command;
        }

        public CommandFactory( DaemonConfiguration configuration )
        {
            Configuration = configuration;
        }
    }
}