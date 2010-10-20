using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.StructureMap;
using Symbiote.Eidetic;
using Symbiote.Log4Net;

namespace MembaseDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Daemon(x => x
                    .DisplayName("membase demo")
                    .Description("membase demo")
                    .Name("membase demo")
                    .Arguments(args))
                //.Redis(x => x.AddServer("10.15.199.65")) //AddLocalServer())
                .Eidetic(x => x.AddLocalServer())
                .AddConsoleLogger<MembaseDemo>(x => x.Debug().MessageLayout(m => m.Message().Newline()))
                .RunDaemon();
        }
    }

    public class MembaseDemo
        : IDaemon
    {
        protected IRemember Membase { get; set; }

        public void Start()
        {
            var singleWrites = new SerialRedisSingleWrites(Membase);
            "Starting serial single write/read tests"
                .ToDebug<MembaseDemo>();
            singleWrites.Execute();

            var parallelWrites = new ParallelRedisSingleWrites(Membase);
            "Starting parallel single write/read tests"
                .ToDebug<MembaseDemo>();
            parallelWrites.Execute();
        }

        public void Stop()
        {

        }

        public MembaseDemo(IRemember membase)
        {
            Membase = membase;
        }
    }

    public class SerialRedisSingleWrites
    {
        protected IRemember Membase { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 10000;
            for (int i = 0; i < iterations; i++)
            {
                var key = i.ToString();
                var original = "value {0}".AsFormat(i);
                Membase.Store(StoreMode.Set, key, original);
                var value = Membase.Get<string>(key);
                if (value != original)
                    throw new Exception("Read fail :( :( :(");
            }
            watch.Stop();
            "Read and wrote {0} records in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<MembaseDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    iterations / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2));
            "Flushing"
                .ToDebug<MembaseDemo>();
            Membase.FlushAll();
        }

        public SerialRedisSingleWrites(IRemember membase)
        {
            Membase = membase;
        }
    }

    public class ParallelRedisSingleWrites
    {
        protected IRemember Membase { get; set; }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            var iterations = 10000;
            Action<int> invoke = ReadWrite;
            var waits = new Stack<IAsyncResult>(iterations);
            for (int i = 0; i < iterations; i++)
            {
                ReadWrite(i);
                waits.Push(invoke.BeginInvoke(i, null, null));
            }
            while (waits.Count > 0)
                waits.Pop().AsyncWaitHandle.WaitOne();

            watch.Stop();
            "Read and wrote {0} records in {1} miliseconds. \r\n\t{2} read/write ops per second.\r\n\t{3} miliseconds per operation."
                .ToDebug<MembaseDemo>(
                    iterations,
                    watch.ElapsedMilliseconds,
                    iterations / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (iterations * 2));
            "Flushing"
                .ToDebug<MembaseDemo>();
            Membase.FlushAll();
        }

        protected void ReadWrite(int i)
        {
            var key = i.ToString();
            var original = "value {0}".AsFormat(i);
            Membase.Store(StoreMode.Set, key, original);
            var value = Membase.Get<string>(key);
            if (value != original)
            {
                "Write / Read operation fail!!!"
                    .ToError<MembaseDemo>();
                throw new Exception("Read fail :( :( :(");
            }
        }

        public ParallelRedisSingleWrites(IRemember membase)
        {
            Membase = membase;
        }
    }
}
