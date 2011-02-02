using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Watcher.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = Path.GetFullPath( @"..\..\..\..\Test" );
            Console.WriteLine( "Watching: {0}", file );
            var watcher = new FileSystemWatcher( );
            watcher.Path = file;
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess;
            watcher.IncludeSubdirectories = true;
            Observable
                .FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                    c => c.Invoke,
                    h =>
                    {
                        watcher.Created += h;
                        watcher.Changed += h;
                        watcher.Deleted += h;
                    },
                    h =>
                    {
                        watcher.Created -= h;
                        watcher.Changed -= h;
                        watcher.Deleted -= h;
                    })
                .Subscribe(e =>
                {
                    Console.WriteLine("CHANGE");
                });
            watcher.EnableRaisingEvents = true;

            Console.ReadKey();
        }

        static void watcher_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("BOOOOO");
        }

        private static void OnSystemChange( object sender, FileSystemEventArgs eventArgs )
        {
            Console.WriteLine("CHANGE");
        }
    }
}
