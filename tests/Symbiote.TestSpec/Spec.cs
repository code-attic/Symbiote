using System.Collections.Generic;
using MbUnit.Framework;

namespace Symbiote.TestSpec
{
    [TestFixture]
    public abstract class Spec
    {
        [SetUp]
        public virtual void SetUp()
        {
            Arrange();
            Act();
        }

        public abstract void Arrange();
        public abstract void Act();

        [TearDown]
        public virtual void TearDown()
        {
        }

        [FixtureSetUp]
        public virtual void Initialize()
        {
        }

        [FixtureTearDown]
        public virtual void Finish()
        {
            var test = new List<string>();

        }
    }
}