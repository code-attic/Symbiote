﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using Symbiote.Core;

namespace Symbiote.Daemon.Args
{
    public class Arguments
    {
        [Option( "i", "install", HelpText = "Install the service as a windows service", MutuallyExclusiveSet = "u" )] 
        public bool Install = false;

        [Option("u", "uninstall", HelpText = "Uninstall an installed windows service", MutuallyExclusiveSet = "i")]
        public bool Uninstall = false;

        [Option("user", "user", HelpText = "Specify the account to run the service under")]
        public string UserName = "";

        [Option("p", "password", HelpText = "Sepcify the password for the account")]
        public string Password = "";

        [OptionArray( "s", "service", HelpText = "Any additional arguments to pass on to the actual hosted service. Not encouraged, but still available." )] 
        public string[] Remaining = null;
    }

    public class ArgumentParser
    {
        public Arguments Parsed { get; set; }

        public ArgumentParser( string[] arguments )
        {
            Parsed = new Arguments();
            var parser = new CommandLineParser();
            parser.ParseArguments( arguments, Parsed );
        }
    }
}