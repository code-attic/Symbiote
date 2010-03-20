using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Hibernate
{
    public interface IRepository<T> where T : class
    {
        void Commit();
        T FindOne<TEntity>(ISearchCriteria<TEntity> entity) where TEntity : T;
        IList<T> FindAll<TEntity>(ISearchCriteria<TEntity> entity) where TEntity : T;
        void Delete<TEntity>(object id) where TEntity : T;
        void Delete<TEntity>(ISearchCriteria<TEntity> criteria) where TEntity : T;
        T Get<TEntity>(object id) where TEntity : T;
        IList<T> GetAll<TEntity>(ISearchCriteria<TEntity> criteria) where TEntity : T;
        int GetCount<TEntity>(ISearchCriteria<TEntity> criteria) where TEntity : T;
        int GetTotal<TEntity>() where TEntity : T;
        void Insert<TEntity>(TEntity entity) where TEntity : T;
        void Insert<TEntity>(IEnumerable<TEntity> list) where TEntity : T;
    }
}
