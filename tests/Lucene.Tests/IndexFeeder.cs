using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Symbiote.Lucene;
using Symbiote.Core.Extensions;

namespace Lucene.Tests
{
    public class IndexFeeder
        : IVisit<Dictionary<string,string>>
    {
        protected ConcurrentBag<IObserver<Tuple<string, string>>> observers { get; set; }

        public IDisposable Subscribe(IObserver<Tuple<string, string>> observer)
        {
            var disposable = observer as IDisposable;
            observers.Add(observer);
            return disposable;
        }

        public void Accept(Dictionary<string, string> root)
        {
            root
                .ForEach(x => observers
                                  .ForEach(y => y.OnNext(Tuple.Create(x.Key, x.Value))));

            observers
                .ForEach(y => y.OnCompleted());
        }

        public IndexFeeder()
        {
            observers = new ConcurrentBag<IObserver<Tuple<string, string>>>();
        }
    }
}
