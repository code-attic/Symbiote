using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate;
using Symbiote.Core.Extensions;
using NHibernate.Linq;

namespace Symbiote.Hibernate.Impl
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ISessionManager _manager;

        public Repository(ISessionManager manager)
        {
            _manager = manager;
        }

        protected IQueryable<TEntity> CreateQuery<TEntity>() where TEntity : T
        {
            var session = GetCurrentSession();
            IQueryable<TEntity> queryable = session.Linq<TEntity>();

            return from entity in queryable
                   select entity;
        }

        protected IQueryable<TEntity> CreateQuery<TEntity>(ISearchCriteria<TEntity> criteria) where TEntity : T
        {
            return CreateQuery<TEntity>()
                .FilterBy(criteria.All)
                .Order(criteria.Order)
                .Page(criteria.PageNumber, criteria.PageSize);

        }

        public ISession GetCurrentSession()
        {
            return _manager.CurrentSession;
        }

        public void Commit()
        {
            _manager.CurrentSession.Flush();
        }

        public void Delete<TEntity>(object id) where TEntity : T
        {
            var entity = Get<TEntity>(id);
            if (entity != null)
                GetCurrentSession().Delete(entity);
        }

        public void Delete<TEntity>(ISearchCriteria<TEntity> criteria) where TEntity : T
        {
            var session = GetCurrentSession();
            FindAll(criteria).ForEach(session.Delete);
        }

        public IList<T> FindAll<TEntity>(ISearchCriteria<TEntity> criteria) where TEntity : T
        {
            return CreateQuery(criteria)
                .Cast<T>()
                .ToList();
        }

        public T FindOne<TEntity>(ISearchCriteria<TEntity> criteria) where TEntity : T
        {
            return CreateQuery(criteria)
                .Take(1)
                .FirstOrDefault();
        }

        public T Get<TEntity>(object id) where TEntity : T
        {
            var criteria = new SearchCriteria<TEntity>();
            var expression = GetIdEqualityPredicate<TEntity>(id);
            criteria.Add(expression);

            return CreateQuery(criteria).FirstOrDefault();
        }

        public IList<T> GetAll<TEntity>(ISearchCriteria<TEntity> criteria) where TEntity : T
        {
            return CreateQuery(criteria)
                .Cast<T>()
                .ToList();
        }

        public int GetCount<TEntity>(ISearchCriteria<TEntity> criteria) where TEntity : T
        {
            return CreateQuery(criteria).Count();
        }

        public int GetTotal<TEntity>() where TEntity : T
        {
            return CreateQuery<TEntity>().Count();
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : T
        {
            var session = GetCurrentSession();
            session.Save(entity);
        }

        public void Insert<TEntity>(IEnumerable<TEntity> list) where TEntity : T
        {
            list.ForEach(Insert);
        }

        private Expression<Func<TEntity, bool>> GetIdEqualityPredicate<TEntity>(object id)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "TEntity");
            var property = GetIdFor<TEntity>();
            var propertyExpr = Expression.Property(parameter, property.Item1);
            var valueExpr = Expression.Constant(id);
            var conversionExpr = Expression.Convert(valueExpr, property.Item2);
            var equalityExpr = Expression.Equal(propertyExpr, conversionExpr);
            return Expression.Lambda<Func<TEntity, bool>>(equalityExpr, parameter);
        }

        private Tuple<string, Type> GetIdFor<TEntity>()
        {
            var metadata = GetCurrentSession().SessionFactory.GetClassMetadata(typeof(TEntity));
            return Tuple.Create(metadata.IdentifierPropertyName, metadata.IdentifierType.ReturnedClass);
        }
    }
}
