using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Symbiote.Core.Utility
{
    public class HierarchyVisitor
        : IObservable<Tuple<Type, string, object>>
    {
        protected ConcurrentBag<IObserver<Tuple<Type, string, object>>> Watchers { get; set; }
        protected Predicate<object> MatchIdentifier { get; set; }

        public void Visit(object instance)
        {
            var members =
                instance.GetType()
                    .GetMembers(
                        BindingFlags.FlattenHierarchy |
                        BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.Instance)
                    .Where(x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property)
                    .Select(x =>
                                {
                                    return "";
                                });
        }

        public IDisposable Subscribe(IObserver<Tuple<Type, string, object>> observer)
        {
            var disposable = observer as IDisposable;
            Watchers.Add(observer);
            return disposable;
        }

        public HierarchyVisitor(Predicate<object> notifyOnMatches)
        {
            Watchers = new ConcurrentBag<IObserver<Tuple<Type, string, object>>>();
        }
    }
}
