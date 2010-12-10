using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.StructureMap;
using Symbiote.Daemon;
using Symbiote.Redis;
using Symbiote.Log4Net;
using Symbiote.Core.Extensions;
using System.Runtime.Serialization;

namespace RedisDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Daemon(x => x
                    .DisplayName("redis demo")
                    .Description("redis demo")
                    .Name("redis demo")
                    .Arguments(args))
                //.Redis(x => x.AddServer("10.15.199.65").LimitPoolConnections(5))
                .Redis(x => x.AddServer("10.15.199.4").LimitPoolConnections(5))
                .AddConsoleLogger<RedisDemo>(x => x.Debug().MessageLayout(m => m.Message().Newline()))
                .RunDaemon();
        }
    }

    public class RedisDemo
        : IDaemon
    {
        protected IRedisClient Redis { get; set; }

        public void Start()
        {
            //var singleWrites = new SerialRedisSingleWrites(Redis);
            //"Starting serial single write/read tests".ToDebug<RedisDemo>();
            //singleWrites.Execute();

            //var parallelWrites = new ParallelRedisSingleWrites(Redis);
            //"Starting parallel single write/read tests".ToDebug<RedisDemo>();
            //parallelWrites.Execute();

            //var bulkWrite = new BulkRedisWrites(Redis);
            //"Starting bulk write, serial reads test".ToDebug<RedisDemo>();
            //bulkWrite.Execute();

            //var singleWrites = new SerialRedisSingleWritesList(Redis);
            //"Starting serial single write/read tests".ToDebug<RedisDemo>();
            //singleWrites.Execute();

            //var singleWrites = new SerialRedisHashMany(Redis);
            //"Starting serial hash multi write/read tests".ToDebug<RedisDemo>();
            //singleWrites.Execute();

            //var singleWrites = new SerialRedisHash(Redis);
            //"Starting serial hash write/read tests".ToDebug<RedisDemo>();
            //singleWrites.Execute();

            //var singleWrites = new SerialRedisHashKeys(Redis);
            //"Starting serial hash keys write/read tests".ToDebug<RedisDemo>();
            //singleWrites.Execute();

            //var singleWrites = new SerialRedisHashVals(Redis);
            //"Starting serial hash values write/read tests".ToDebug<RedisDemo>();
            //singleWrites.Execute();

            var singleWrites = new SerialRedisSets(Redis);
            "Starting serial set write/read tests".ToDebug<RedisDemo>();
            singleWrites.Execute();

            //var singleMultiWrites = new SerialRedisSetsMulti(Redis);
            //"Starting serial set multi write/read tests".ToDebug<RedisDemo>();
            //singleMultiWrites.Execute();
            
        }

        public void Stop()
        {

        }

        public RedisDemo(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class SerialRedisSingleWrites
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 1000000;
            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                var original = new Tx(i);
                Redis.Set(key, original);
                var value = Redis.Get<Tx>(key);
                if (!value.Equals(original))
                {
                    "Write / Read operation fail!!!"
                        .ToError<RedisDemo>();
                    throw new Exception("Read fail :( :( :(");
                }
            }
            watch.Stop();
            "Read and wrote {0} records in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * 2) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2));
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        public SerialRedisSingleWrites(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class BulkRedisWrites
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            Redis.FlushAll();

            var iterations = 10000;
            List<Tx> transactions = new List<Tx>(iterations);
            for (int i = 0; i < iterations; i++)
            {
                transactions.Add(new Tx(i));
            }
            var watch = Stopwatch.StartNew();
            Redis.Set(transactions, t => t.FileCheckSum.ToString());
            watch.Stop();
            "Wrote {0} records in {1} miliseconds"
                .ToDebug<RedisDemo>(iterations, watch.ElapsedMilliseconds);

            watch.Start();

            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                var value = Redis.Get<Tx>(key);
                if (!value.Equals(transactions[i]))
                {
                    "Write / Read operation fail!!!"
                        .ToError<RedisDemo>();
                    throw new Exception("Read fail :( :( :(");
                }
            }

            watch.Stop();
            "Read and wrote {0} records in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * 2) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2));
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        public BulkRedisWrites(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class ParallelRedisSingleWrites
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 100000;
            Action<int> invoke = ReadWrite;
            var waits = new Stack<IAsyncResult>(iterations);
            var observable =
                waits.ToObservable().DoWhile(() => waits.Count > 5).Subscribe(x => x.AsyncWaitHandle.WaitOne());

            for (int i = 0; i < iterations; i++)
            {
                ReadWrite(i);
                waits.Push(invoke.BeginInvoke(i, null, null));
            }
            while (waits.Count > 0)
                waits.Pop().AsyncWaitHandle.WaitOne();

            watch.Stop();
            "Read and wrote {0} records in {1} miliseconds. \r\n\t{2} ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * 2) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2));
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        protected void ReadWrite(int i)
        {
            var key = i.ToString();
            var original = new Tx(i);
            Redis.Set(key, original);
            var value = Redis.Get<Tx>(key);
            if (!value.Equals(original))
            {
                "Write / Read operation fail!!!"
                    .ToError<RedisDemo>();
                throw new Exception("Read fail :( :( :(");
            }
        }

        public ParallelRedisSingleWrites(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class SerialRedisListRange
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 1000;
            var elements = 20;
            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                for (int j = 0; j < elements; j++)
                {
                    var original = new Tx(i*iterations + j);
                    Redis.RPush(key, original);
                }
                var rng = Redis.LRange<Tx>(key, 0, elements);
            }
            watch.Stop();
            "Read and wrote {0} lists with {4} elements each in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * 2 * elements) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2 * elements),
                    elements);
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        public SerialRedisListRange(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class SerialRedisSingleWritesList
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 10000;
            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                var original = new Tx(i);
                Redis.LPush(key, original);
                var value = Redis.RPop<Tx>(key);
                if (!value.Equals(original))
                {
                    "Write / Read operation fail!!!"
                        .ToError<RedisDemo>();
                    throw new Exception("Read fail :( :( :(");
                }
            }
            watch.Stop();
            "Read and wrote {0} records in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * 2) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2));
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        public SerialRedisSingleWritesList(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class SerialRedisHashMany
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 1000;
            var elements = 20;
            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                var originalDict = new Dictionary<string, Tx>();
                var getList = new List<string>();
                for (int j = 0; j < elements; j++)
                {
                    var original = new Tx(i * iterations + j);
                    var field = j.ToString();
                    originalDict.Add(field, original);
                    getList.Add(field);

                }
                Redis.HSet<Tx>(key, originalDict);
                var newList = Redis.HGet<Tx>(key, getList);
            }
            watch.Stop();
            "Read and wrote {0} lists with {4} elements each in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * 2 * elements) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2 * elements),
                    elements);
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        public SerialRedisHashMany(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class SerialRedisHash
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 1000;
            var elements = 20;
            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                for (int j = 0; j < elements; j++)
                {
                    var original = new Tx(i * iterations + j);
                    var field = j.ToString();
                    Redis.HSet(key, field, original);
                    var returned = Redis.HGet<Tx>(key, field);
                    //List<string> lst = new List<string>();
                    //lst.Add(field);
                    //var returned = Redis.HGet<Tx>(key, lst);
                }
            }
            watch.Stop();
            "Read and wrote {0} lists with {4} elements each in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * 2 * elements) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2 * elements),
                    elements);
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        public SerialRedisHash(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class SerialRedisHashKeys
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 1000;
            var elements = 20;
            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                var originalDict = new Dictionary<string, Tx>();
                for (int j = 0; j < elements; j++)
                {
                    var original = new Tx(i * iterations + j);
                    var field = j.ToString();
                    originalDict.Add(field, original);

                }
                Redis.HSet<Tx>(key, originalDict);
                var keyList = Redis.HKeys(key);
            }
            watch.Stop();
            "Wrote {0} lists with {4} elements each and got keys in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * elements) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * elements),
                    elements);
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        public SerialRedisHashKeys(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class SerialRedisHashVals
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 1000;
            var elements = 20;
            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                var originalDict = new Dictionary<string, Tx>();
                for (int j = 0; j < elements; j++)
                {
                    var original = new Tx(i * iterations + j);
                    var field = j.ToString();
                    originalDict.Add(field, original);

                }
                Redis.HSet<Tx>(key, originalDict);
                var keyList = Redis.HVals<Tx>(key);
            }
            watch.Stop();
            "Wrote {0} lists with {4} elements each and got keys in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * elements) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * elements),
                    elements);
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        public SerialRedisHashVals(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class SerialRedisSets
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 1000;
            var elements = 20;
            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                for (int j = 0; j < elements; j++)
                {
                    var original = new Tx(i * iterations + j);
                    var field = j.ToString();
                    Redis.SAdd(key, original);
                    var returned = Redis.SPop<Tx>(key);
                }
            }
            watch.Stop();
            "Read and wrote {0} sets with {4} elements each in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * 2 * elements) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2 * elements),
                    elements);
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        public SerialRedisSets(IRedisClient redis)
        {
            Redis = redis;
        }
    }

    public class SerialRedisSetsMulti
    {
        protected IRedisClient Redis { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 1000;
            var elements = 20;
            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                var vals = new List<Tuple<string,Tx>>();
                for (int j = 0; j < elements; j++)
                {
                    var original = new Tx(i * iterations + j);
                    vals.Add(new Tuple<string, Tx>(key,original));

                }
                var addResults = Redis.SAdd(vals);
                //var returned = Redis.SPop<Tx>(key);
            }
            watch.Stop();
            "Read and wrote {0} sets with {4} elements each in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<RedisDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    (iterations * 2 * elements) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2 * elements),
                    elements);
            "Flushing"
                .ToDebug<RedisDemo>();
            Redis.FlushAll();
        }

        public SerialRedisSetsMulti(IRedisClient redis)
        {
            Redis = redis;
        }
    }


    [DataContract]
    public class Tx
    {
        [DataMember(Name = "FileName", Order = 1, IsRequired = false)]
        public string FileName { get; set; }
        [DataMember(Name = "FileCheckSum", Order = 2, IsRequired = false)]
        public int FileCheckSum { get; set; }
        [DataMember(Name = "LineCheckSum", Order = 3, IsRequired = false)]
        public int LineCheckSum { get; set; }
        [DataMember(Name = "Name", Order = 4, IsRequired = false)]
        public string Name { get; set; }
        [DataMember(Name = "CustomerName", Order = 5, IsRequired = false)]
        public string CustomerName { get; set; }
        [DataMember(Name = "BankO", Order = 6, IsRequired = false)]
        public string BankO { get; set; }
        [DataMember(Name = "Routing", Order = 7, IsRequired = false)]
        public string Routing { get; set; }
        [DataMember(Name = "Account", Order = 8, IsRequired = false)]
        public string Account { get; set; }
        [DataMember(Name = "NachaId", Order = 9, IsRequired = false)]
        public string NachaId { get; set; }
        [DataMember(Name = "Submitted", Order = 10, IsRequired = false)]
        public DateTime Submitted { get; set; }

        public Tx()
        {
            
        }
        public Tx(int checksum)
        {
            FileName = "Test.txt";
            FileCheckSum = checksum;
            LineCheckSum = 1;
            Name = "Empty";
            CustomerName = "Empty";
            NachaId = "101010101";
            BankO = "This is a Bank Name and it's Supar Totally Long and Really hard to Remember";
            Routing = "001001001";
            Account = "1001004411";
            Submitted = DateTime.Now;
        }

        public override int GetHashCode()
        {
            return FileCheckSum;
        }

        public override bool Equals(object obj)
        {
            var compare = obj as Tx;
            return compare != null && compare.FileCheckSum == FileCheckSum;
        }
    }

}
