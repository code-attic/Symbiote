using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Symbiote.Core.Extensions;

namespace Symbiote.Hibernate.Impl
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> FilterBy<T>(this IQueryable<T> query, IEnumerable<Expression<Func<T, bool>>> criteria)
        {
            foreach (var criterion in criteria.Where(c => c != null))
            {
                query = query.Where(criterion);
            }
            return query;
        }

        public static IQueryable<T> Order<T>(this IQueryable<T> query, IEnumerable<Tuple<string, SortOrder>> orderCriteria)
        {
            if (orderCriteria.Count() == 0)
                return query;

            var orderedQuery = OrderQuery(orderCriteria.First(), query as IOrderedQueryable<T>, true);
            orderCriteria
                .Skip(1)
                .ForEach(t => orderedQuery = OrderQuery(t, orderedQuery, false));

            return orderedQuery;
        }

        private static IOrderedQueryable<T> OrderQuery<T>(Tuple<string, SortOrder> sortCriteria, IOrderedQueryable<T> query, bool firstRun)
        {
            var typeParameter = Expression.Parameter(typeof(T), "entity");
            var access = Expression.Property(typeParameter, sortCriteria.Item1);

            var lambda = Expression.Lambda<Func<T, object>>(
                Expression.Convert(access, typeof(object)), typeParameter);

            if (sortCriteria.Item2 == SortOrder.Ascending)
            {
                return firstRun ? query.OrderBy(lambda) : query.ThenBy(lambda);
            }
            else
            {
                return firstRun ? query.OrderByDescending(lambda) : query.ThenByDescending(lambda);
            }
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> query, int? pageNumber, int? pageSize)
        {
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                if (pageNumber == 0)
                    throw new ArgumentOutOfRangeException("pageNumber", 0, "There is no page 0");

                var skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }
            return query;
        }
    }
}
