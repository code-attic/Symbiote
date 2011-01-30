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
using CommandLine;

namespace Symbiote.Daemon.Args
{
    public class Arguments
    {
        [Option( "i", "install", HelpText = "Install the service as a windows service" )] public bool Install;

        [Option( "p", "password", HelpText = "Sepcify the password for the account" )] public string Password = "";

        [OptionArray( "s", "service",
            HelpText =
                "Any additional arguments to pass on to the actual hosted service. Not encouraged, but still available."
            )] public string[] Remaining;

        [Option( "u", "uninstall", HelpText = "Uninstall an installed windows service" )] public bool Uninstall;

        [Option( "a", "account", HelpText = "Specify the account to run the service under" )] public string UserName =
            "";
    }
}