using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Symbiote.Hibernate
{
    public interface ISearchCriteria<T>
    {
        int? PageNumber { get; }
        int? PageSize { get; }

        ISearchCriteria<T> Add(Expression<Func<T, bool>> criteria);
        ISearchCriteria<T> PageBy(int pageNumber, int pageSize);
        ISearchCriteria<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> orderBy, SortOrder order);
        IEnumerable<Expression<Func<T, bool>>> All { get; }
        IEnumerable<Tuple<string, SortOrder>> Order { get; }
    }
}