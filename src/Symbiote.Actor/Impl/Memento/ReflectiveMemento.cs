using System;
using System.Collections.Generic;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;
using Symbiote.Core.Utility;

namespace Symbiote.Actor.Impl.Memento
{
    /// <summary>
    /// Behold and weep. Exists to motivate developers
    /// to write their own concrete mementos because if they don't
    /// this provides an awful default that will suck the performance
    /// right out of their app.
    /// </summary>
    public class ReflectiveMemento<T>
        : IMemento<T>,
          IObserver<Tuple<object, string, object>>
    {
        public Dictionary<string, object> MemberValues { get; set; }
        public HierarchyVisitor Visitor { get; set; }

        public void Capture( T instance )
        {
            MemberValues.Clear();
            Visitor.Visit( instance );
        }

        public void Reset( T instance )
        {
            MemberValues
                .ForEach( x => Reflector.WriteMember( instance, x.Key, x.Value ) );
        }

        public void OnNext( Tuple<object, string, object> value )
        {
            MemberValues.Add( value.Item2, value.Item3 );
        }

        public void OnError( Exception error )
        {
            //do nothing
        }

        public void OnCompleted()
        {
            //do nothing
        }

        public T Retrieve()
        {
            var instance = Assimilate.GetInstanceOf<T>();
            Reset(instance);
            return instance;
        }

        public ReflectiveMemento()
        {
            MemberValues = new Dictionary<string, object>();
            Visitor = new HierarchyVisitor(true, x => true);
            Visitor.Subscribe(this);
        }
    }
}