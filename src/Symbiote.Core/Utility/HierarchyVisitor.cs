/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;

namespace Symbiote.Core.Utility
{
    public class HierarchyVisitor
        : IObservable<Tuple<object, string, object>>
    {
        protected List<IObserver<Tuple<object, string, object>>> Watchers { get; set; }
        protected Predicate<object> MatchIdentifier { get; set; }

        public void Visit(object instance)
        {
            if(MatchIdentifier(instance))
                NotifyWatchers(null, null, instance);

            if (IsCrawlableEnumerable(instance))
                Visit(null, "", instance);
            else
                Crawl(instance);
        }

        protected void Crawl(object instance)
        {
            Reflector
                .GetProperties(instance.GetType())
                .Where(x => !x.PropertyType.IsValueType)
                .ForEach(x => Visit(instance, x.Name, Reflector.ReadMember(instance, x.Name)));
        }

        protected void Visit(object parent, string member, object value)
        {
            if(value != null)
            {
                if(IsCrawlableEnumerable(value))
                {
                    try
                    {
                        var enumerator = (value as IEnumerable).GetEnumerator();
                        enumerator.MoveNext();
                        do
                        {
                            if(enumerator.Current != null)
                                Visit(parent, member, enumerator.Current);
                        } while (enumerator.MoveNext());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    if (MatchIdentifier(value))
                        NotifyWatchers(parent, member, value);
                    Crawl(value);
                }
            }
        }

        protected bool IsCrawlableEnumerable(object value)
        {
            return value.GetType().GetInterface("IEnumerable", false) != null && value.GetType().Name != "String";
        }

        protected void NotifyWatchers(object parent, string member, object instance)
        {
            Watchers.ForEach(x => x.OnNext(Tuple.Create(parent, member, instance)));
        }

        public IDisposable Subscribe(IObserver<Tuple<object, string, object>> observer)
        {
            var disposable = observer as IDisposable;
            Watchers.Add(observer);
            return disposable;
        }

        public HierarchyVisitor(Predicate<object> notifyOnMatches)
        {
            MatchIdentifier = notifyOnMatches;
            Watchers = new List<IObserver<Tuple<object, string, object>>>();
        }
    }
}
