using System;

namespace Symbiote.Lucene
{
    public interface IVisit<T> : IObservable<Tuple<string, string>>
    {
        void Accept(T root);
    }
}