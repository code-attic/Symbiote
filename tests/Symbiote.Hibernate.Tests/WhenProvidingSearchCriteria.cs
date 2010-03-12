using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MbUnit.Framework;
using Symbiote.Core;
using Symbiote.TestSpec;
using SortOrder=System.Data.SqlClient.SortOrder;

namespace Symbiote.Hibernate.Tests
{
    public class WhenProvidingSearchCriteria : Spec
    {
        public override void Arrange()
        {
            
        }

        public override void Act()
        {
            
        }

        [Assert]
        public void ShouldSupportPaging()
        {
            var criteria = new SearchCriteria<TestEntity>();
            criteria.PageBy(2, 25);
            Assert.AreEqual(2, criteria.PageNumber);
            Assert.AreEqual(25, criteria.PageSize);
        }

        [Test]
        public void ShouldAllowAddingOfCriteria()
        {
            var criteria = new SearchCriteria<TestEntity>();
            criteria.Add(entity => entity.Id.HasValue);
            var list = new List<Expression<Func<TestEntity, bool>>>(criteria.All);
            Assert.IsTrue(list.Count == 1);
            Assert.AreEqual("entity.Id.HasValue", list[0].Body.ToString());
        }

        [Test]
        public void ShouldAllowAddingOfOrder()
        {
            var criteria = Select.Where<TestEntity>(t => t.Id > 0);
            criteria.OrderBy(t => t.Id, SortOrder.Descending);
            var order = criteria.Order.ToList();
            Assert.AreEqual(1, order.Count());
            Assert.AreEqual(Tuple.Create("Id", SortOrder.Descending), order.First());
        }
    }
}