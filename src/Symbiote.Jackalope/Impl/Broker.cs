using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Symbiote.Jackalope.Impl
{
    public class Broker : BaseBroker
    {
        protected bool Run { get; set; }

        public override void Start(string queueName)
        {
            _queueName = queueName;
            Run = true;
            var tasks = new List<Task>();
            var threshold = 5;
            while (Run)
            {
                while(tasks.Count < threshold)
                {
                    var task = Task
                        .Factory
                        .StartNew(Dispatch);
                    tasks.Add(task);
                }

                tasks.RemoveAll(x => x.Status == TaskStatus.RanToCompletion);
            }
        }

        public override void Stop()
        {
            Run = false;
            Console.WriteLine("Killing broker...");
        }

        public Broker(IChannelProxyFactory proxyFactory, IMessageSerializer messageSerializer) : base(proxyFactory, messageSerializer)
        {
        }
    }
}
