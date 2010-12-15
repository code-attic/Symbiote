using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using Symbiote.Core;

namespace Symbiote.Daemon.Args
{
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
