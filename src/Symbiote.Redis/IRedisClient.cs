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