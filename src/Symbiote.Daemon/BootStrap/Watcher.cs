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
using System.Collections.Generic;
using System.IO;
using Symbiote.Core.Utility;
using Symbiote.Daemon.BootStrap.Config;
using System.Linq;
using Symbiote.Messaging;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.BootStrap
{
    public class Watcher
        : IDisposable
    {
        public BootStrapConfiguration Configuration { get; set; }
        public IList<FileSystemWatcher> SystemEvents { get; set; }
        public IList<IDisposable> SystemObservers { get; set; }
        public IList<FileSystemWatcher> Watchers { get; set; }
        public IBus Bus { get; set; }

        public Watcher( BootStrapConfiguration configuration, IBus bus )
        {
            Configuration = configuration;
            SystemObservers = new List<IDisposable>();
            Watchers = new List<FileSystemWatcher>();
            Bus = bus;
        }
        
        public void ConfigureWatcher(string path)
        {
            //Watchers.Add( CreateDirectoryWatcher( path ) );
            Watchers.Add( CreateFileWatcher( path ) );
        }

        public FileSystemWatcher CreateDirectoryWatcher(string file)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = file;
            watcher.NotifyFilter = NotifyFilters.DirectoryName;
            watcher.IncludeSubdirectories = true;
            var observer = Observable
                .FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                    c => c.Invoke,
                    h =>
                    {
                        watcher.Changed += h;
                        watcher.Deleted += h;
                    },
                    h =>
                    {
                        watcher.Changed -= h;
                        watcher.Deleted -= h;
                    })
                .Do(x =>
                {
                    var path = GetPathFromEvent(x);
                    if(x.EventArgs.ChangeType == WatcherChangeTypes.Deleted)
                    {
                        OnApplicationDeletion( path );
                    }
                    else
                    {
                        OnNewApplication( path );
                    }
                })
                .Subscribe();
            SystemObservers.Add( observer );
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        public FileSystemWatcher CreateFileWatcher(string file)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = file;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.IncludeSubdirectories = true;
            var observer = Observable
                .FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                    c => c.Invoke,
                    h =>
                    {
                        watcher.Changed += h;
                        watcher.Deleted += h;
                    },
                    h =>
                    {
                        watcher.Changed -= h;
                        watcher.Deleted -= h;
                    })
                .Where(x => !string.IsNullOrEmpty(Path.GetExtension(x.EventArgs.FullPath)))
                .Throttle( TimeSpan.FromSeconds( 5 ) )
                .DistinctUntilChanged(GetPathFromEvent)
                .Do(x =>
                    {
                        var path = GetPathFromEvent( x );
                        if (x.EventArgs.ChangeType == WatcherChangeTypes.Deleted)
                        {
                            OnApplicationDeletion(path);
                        }
                        else
                        {
                            OnApplicationChange(path);
                        }
                    })
                .Subscribe();
            SystemObservers.Add(observer);
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        public string GetPathFromEvent( IEvent<FileSystemEventArgs> fileSystemEvent )
        {
            var fileName = Path.GetFileName(fileSystemEvent.EventArgs.FullPath);
            return string.IsNullOrEmpty(Path.GetExtension(fileName))
                       ? fileSystemEvent.EventArgs.FullPath
                       : fileSystemEvent.EventArgs.FullPath.Replace(fileName, "").TrimEnd(Path.DirectorySeparatorChar);
        }

        public string IsPathToAFile(FileSystemEventArgs e)
        {
            return Path.GetExtension(e.Name);
        }

        public void OnNewApplication(string path)
        {
            Bus.Publish("local", new NewApplication() { DirectoryPath = Path.GetFullPath(path) });
        }

        public void OnApplicationChange(string path)
        {
            Bus.Publish("local", new ApplicationChanged() { DirectoryPath = Path.GetFullPath(path) });
        }

        public void OnApplicationDeletion(string path)
        {
            Bus.Publish("local", new ApplicationDeleted() { DirectoryPath = Path.GetFullPath(path) });
        }

        public void Start()
        {
            Configuration.WatchPaths.ForEach(ConfigureWatcher);
        }

        public void Dispose()
        {
            SystemObservers.Clear();
            Watchers.ForEach( x =>
                                  {
                                      x.EnableRaisingEvents = false;
                                      x.Dispose();
                                  } );
            Watchers.Clear();
        }
    }
}