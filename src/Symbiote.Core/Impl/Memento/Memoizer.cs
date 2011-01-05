using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace Symbiote.Core.Impl.Memento
{
    public class Memoizer
        : IMemoizer
    {
        protected ConcurrentDictionary<Type, Func<IMemento>> Factories { get; set; }

        public Func<IMemento> GetFactoryFor<T>()
        {
            return Factories.GetOrAdd( typeof(T), BuildFactoryFor<T>() );
        }

        public Func<IMemento> BuildFactoryFor<T>()
        {
            var type = Assimilate.Assimilation.DependencyAdapter.GetTypesRegisteredFor<IMemento<T>>().First();
            var constructor = Expression.New( type );
            var lambda = Expression.Lambda<Func<IMemento>>( constructor, null );
            return lambda.Compile();
        }

        public IMemento<T> GetMemento<T>( T instance )
        {
            //var memento = GetFactoryFor<T>()() as IMemento<T>;
            var memento = Assimilate.GetInstanceOf<IMemento<T>>();
            memento.Capture( instance );
            return memento;
        }

        public T GetFromMemento<T>( IMemento<T> memento )
        {
            return memento.Retrieve();
        }

        public void ResetToMemento<T>( T instance, IMemento<T> memento )
        {
            memento.Reset( instance );
        }
    }
}