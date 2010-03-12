using System;
using System.Linq.Expressions;

namespace Symbiote.Hibernate
{
    public static class Select
    {
        public static ISearchCriteria<T> Where<T>(Expression<Func<T, bool>> criteria)
        {
            var search = new SearchCriteria<T>();
            search.Add(criteria);
            return search;
        }

        public static ISearchCriteria<T> All<T>()
        {
            return new SearchCriteria<T>();
        }
    }
}