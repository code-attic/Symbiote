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
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Diagnostics;
using System.Linq;
using Symbiote.Redis.Impl.Command;
using Symbiote.Redis.Impl.Config;
using Symbiote.Redis.Impl.Serialization;

namespace Symbiote.Redis.Impl
{
    public class RedisClient
        : IDisposable, IRedisClient
    {
        protected RedisConfiguration Configuration { get; set; }
        protected ICacheSerializer Serializer { get; set; }
        protected int _dbInstance = 0;
        
        public int DbInstance 
        {
            get { return _dbInstance; }
		    set 
            {
			    _dbInstance = value;
                var command = new SetDatabaseCommand(value);
                command.Execute();
            }
	    }

        public int DbSize
        {
            get
            {
                var command = new DatabaseSizeCommand();
                return command.Execute();
            }
        }

        public string[] Keys
        {
            get
            {
                var command = new KeyListCommand();
                return command.Execute().ToArray();
            }
        }

        public DateTime LastSave
        {
            get
            {
                var command = new LastSaveCommand();
                return command.Execute();
            }
        }

	    public bool Set<T>(string key, T value)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");
            if (ReferenceEquals(value, default(T)))
			    throw new ArgumentNullException ("value");
	        var command = new SetValueCommand<T>(key, value);
	        return command.Execute();
	    }

        public bool Set<T>(IDictionary<string, T> pairs)
        {
            var set = new SetManyCommand<T>(pairs.Select(p => Tuple.Create(p.Key, p.Value)));
            return set.Execute();
        }

        public bool Set<T>(IEnumerable<T> values, Func<T, string> getKey)
        {
            var set = new SetManyCommand<T>(values.Select(v => Tuple.Create(getKey(v), v)));
            return set.Execute();
        }

        public bool CheckAndSet<T>(string key, T value)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");
		    if (ReferenceEquals(value, default(T)))
			    throw new ArgumentNullException ("value");

	        var command = new CheckAndSetCommand<T>(key, value);
	        return command.Execute();
	    }

	    public T Get<T>(string key)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");

	        var command = new GetCommand<T>(key);
	        return command.Execute();
	    }

	    public T GetSet<T>(string key, T value)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");
		    if (ReferenceEquals(value, default(T)))
			    throw new ArgumentNullException ("value");

	        var command = new GetAndSetCommand<T>(key, value);
	        return command.Execute();
	    }

	    public bool Remove(params string [] args)
	    {
		    if (args == null)
			    throw new ArgumentNullException ("args");
	        var command = new RemoveCommand(args);
	        return command.Execute();
	    }

	    public int Increment(string key)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");
	        var command = new IncrementCommand(1, key);
	        return command.Execute();
	    }

	    public int IncrementBy(string key, int count)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");
            var command = new IncrementCommand(count, key);
            return command.Execute();
	    }

	    public int Decrement(string key)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");
            var command = new DecrementCommand(1, key);
            return command.Execute();
	    }

	    public int Decrement(string key, int count)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");
            var command = new DecrementCommand(count, key);
            return command.Execute();
	    }

	    public bool Rename(string oldKeyname, string newKeyname)
	    {
		    if (oldKeyname == null)
			    throw new ArgumentNullException ("oldKeyname");
		    if (newKeyname == null)
			    throw new ArgumentNullException ("newKeyname");
	        var command = new RenameCommand(oldKeyname, newKeyname);
	        return command.Execute();
	    }

	    public bool Expire(string key, int seconds)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");
	        var command = new ExpireCommand(key, seconds);
	        return command.Execute();
	    }

	    public bool ExpireAt(string key, DateTime time)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");
            var command = new ExpireCommand(key, time);
            return command.Execute();
	    }

	    public int TimeToLive(string key)
	    {
		    if (key == null)
			    throw new ArgumentNullException ("key");
	        var command = new TimeToLiveCommand(key);
	        return command.Execute();
	    }

	    public void Save()
	    {
	        var command = new SaveDatabaseCommand(true);
	        command.Execute();
	    }

	    public void BackgroundSave()
	    {
            var command = new SaveDatabaseCommand(false);
            command.Execute();
	    }

	    public void Shutdown()
	    {
	        var command = new ShutdownCommand();
	        command.Execute();
	    }

	    public void FlushAll()
	    {
	        var command = new FlushCommand(true);
	        command.Execute();
	    }

	    public void FlushDb()
	    {
            var command = new FlushCommand(false);
            command.Execute();
	    }

	    public Dictionary<string,string> GetInfo()
	    {
	        var command = new InfoCommand();
	        return command.Execute();
	    }

	    public string [] GetKeys(string pattern)
	    {
            var command = new KeyListCommand(pattern);
            return command.Execute().ToArray();
	    }

	    public RedisClient(RedisConfiguration configuration, ICacheSerializer serializer)
        {
            Configuration = configuration;
            Serializer = serializer;
        }

        public void Dispose ()
	    {
		    Dispose (true);
		    GC.SuppressFinalize (this);
	    }

	    ~RedisClient ()
	    {
		    Dispose (false);
	    }

	    protected virtual void Dispose (bool disposing)
	    {
		    if (disposing){
		    }
	    }
    }
}
