using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using MbUnit.Framework;
using Symbiote.TestSpec;

namespace Symbiote.Hibernate.Tests
{
    public class WhenSelecting : Spec
    {
        public override void Arrange()
        {
            
        }

        public override void Act()
        {
            
        }

        [Assert]
        public void ShouldReturnSearchCriteria()
        {
            ISearchCriteria<TestEntity> criteria = Select.Where<TestEntity>(entity => entity.Id.HasValue);
            var list = new List<Expression<Func<TestEntity, bool>>>(criteria.All);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("entity.Id.HasValue", list[0].Body.ToString());
        }
    }

    public class TestEntity
    {
        public int? Id { get; set; }
    }
}
