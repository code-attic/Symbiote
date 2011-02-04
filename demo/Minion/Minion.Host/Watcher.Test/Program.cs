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
            FileSystemWatcher watcher = CreateDirectoryWatcher(file);
            FileSystemWatcher watcher2 = CreateFileWatcher(file);

            Console.ReadKey();
        }

        private static FileSystemWatcher CreateDirectoryWatcher( string file ) 
        {
            var watcher = new FileSystemWatcher( );
            watcher.Path = file;
            watcher.NotifyFilter = NotifyFilters.DirectoryName;
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
                .Do( x =>
                {
                    var f = Path.GetFileName( x.EventArgs.FullPath );
                    var p = string.IsNullOrEmpty( Path.GetExtension( f ) )
                                ? x.EventArgs.FullPath
                                : x.EventArgs.FullPath.Replace( f, "" ).TrimEnd( Path.DirectorySeparatorChar );
                    Console.WriteLine( p );
                } )
                .Subscribe();
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private static FileSystemWatcher CreateFileWatcher(string file)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = file;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.IncludeSubdirectories = true;
            Observable
                .FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                    c => c.Invoke,
                    h =>
                    {
                        watcher.Changed += h;
                    },
                    h =>
                    {
                        watcher.Changed -= h;
                    } )
                .Where( x => !string.IsNullOrEmpty( Path.GetExtension( x.EventArgs.FullPath ) ) )
                .BufferWithTime( TimeSpan.FromMinutes( 1 ) )
                .Do(o => o
                    .DistinctUntilChanged( x =>
                    {
                        var f = Path.GetFileName( x.EventArgs.FullPath );
                        var p = string.IsNullOrEmpty( Path.GetExtension( f ) )
                                    ? x.EventArgs.FullPath
                                    : x.EventArgs.FullPath.Replace( f, "" ).TrimEnd( Path.DirectorySeparatorChar );
                        return p;
                    } )
                    .Do( x =>
                    {
                        var f = Path.GetFileName( x.EventArgs.FullPath );
                        var p = string.IsNullOrEmpty( Path.GetExtension( f ) )
                                    ? x.EventArgs.FullPath
                                    : x.EventArgs.FullPath.Replace( f, "" ).TrimEnd( Path.DirectorySeparatorChar );
                        Console.WriteLine( p );
                    } )
                    .Subscribe())
                .Subscribe();
            watcher.EnableRaisingEvents = true;
            return watcher;
        }
    }
}
