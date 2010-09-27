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
using System.Collections.Generic;

namespace Symbiote.Redis
{
    public interface IRedisClient
    {
        bool Set<T>(string key, T value);
        bool Set<T>(IDictionary<string, T> pairs);
        bool Set<T>(IEnumerable<T> values, Func<T, string> getKey);
        bool CheckAndSet<T>(string key, T value);
        T Get<T>(string key);
        T GetSet<T>(string key, T value);
        bool Remove(params string [] args);
        int Increment(string key);
        int IncrementBy(string key, int count);
        int Decrement(string key);
        int Decrement(string key, int count);
        bool Rename(string oldKeyname, string newKeyname);
        bool Expire(string key, int seconds);
        bool ExpireAt(string key, DateTime time);
        int TimeToLive(string key);
        void Save();
        void BackgroundSave();
        void Shutdown();
        void FlushAll();
        void FlushDb();
        Dictionary<string,string> GetInfo();
        string [] GetKeys(string pattern);
    }
}