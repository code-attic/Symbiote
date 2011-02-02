using System;
using System.Collections.Generic;
using System.Globalization;
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
            FileSystemWatcher watcher = CreateWatcher( file );

            watcher.EnableRaisingEvents = true;

            Console.ReadKey();
        }

        private static FileSystemWatcher CreateWatcher( string file ) 
        {
            var watcher = new FileSystemWatcher( );
            watcher.Path = file;
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName;
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
                    } )
                .BufferWithTime( TimeSpan.FromSeconds( 10 ) )
                .Do( o =>
                     o.DistinctUntilChanged( x =>
                     {
                         var f = Path.GetFileName(x.EventArgs.FullPath);
                         var p =
                             string.IsNullOrEmpty(Path.GetExtension(f)) ?
                                                                            x.EventArgs.FullPath :
                                                                                                     x.EventArgs.FullPath.Replace(f, "").TrimEnd(Path.DirectorySeparatorChar);
                         return p;
                     } )
                         .Do( x =>
                         {
                             var f = Path.GetFileName(x.EventArgs.FullPath);
                             var p =
                                 string.IsNullOrEmpty(Path.GetExtension(f)) ?
                                                                                x.EventArgs.FullPath :
                                                                                                         x.EventArgs.FullPath.Replace(f, "").TrimEnd(Path.DirectorySeparatorChar);
                             Console.WriteLine( p );
                         } )
                         .Subscribe(  )
                )
                .Subscribe();
            return watcher;
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
