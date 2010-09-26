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
                .Redis(x => x.AddServer("192.168.1.102").LimitPoolConnections(5))
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

            var bulkWrite = new BulkRedisWrites(Redis);
            "Starting bulk write, serial reads test".ToDebug<RedisDemo>();
            bulkWrite.Execute();
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

    public class Tx
    {
        public string FileName { get; set; }
        public int FileCheckSum { get; set; }
        public int LineCheckSum { get; set; }
        public string Name { get; set; }
        public string CustomerName { get; set; }
        public string BankO { get; set; }
        public string Routing { get; set; }
        public string Account { get; set; }
        public string NachaId { get; set; }
        public DateTime Submitted { get; set; }

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
