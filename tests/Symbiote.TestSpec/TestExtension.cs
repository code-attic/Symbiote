using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;

namespace Symbiote.TestSpec
{
    public static class TestExtension
    {
        public static void ShouldBeNullOrDefault<T>(this T original)
        {
            original.ShouldEqual(default(T));
        }

        public static void ShoulbNotBeNullOrDefault<T>(this T original)
        {
            original.ShouldNotEqual(default(T));
        }

        public static void ShouldBeTrue(this bool flag)
        {
            Assert.IsTrue(flag);
        }

        public static void ShouldBeFalse(this bool flag)
        {
            Assert.IsFalse(flag);
        }

        public static void ShouldBeEmpty<T>(this IEnumerable<T> original)
        {
            Assert.IsTrue(original.Count() == 0);
        }

        public static void ShouldNotBeEmpty<T>(this IEnumerable<T> original)
        {
            Assert.IsTrue(original.Count() > 0);
        }

        public static void ShouldEqual<T>(this T original, T test)
        {
            Assert.AreEqual(original, test);
        }

        public static void ShouldNotEqual<T>(this T original, T test)
        {
            Assert.AreNotEqual(original, test);
        }

        public static void ShouldBeGreaterThan<T>(this T original, T test)
        {
            Assert.GreaterThan(original, test);
        }

        public static void ShouldBeGreaterOrEqualTo<T>(this T original, T test)
        {
            Assert.GreaterThanOrEqualTo(original, test);
        }

        public static void ShouldBeLessThan<T>(this T original, T test)
        {
            Assert.LessThan(original, test);
        }

        public static void ShouldBeLessOrEqualTo<T>(this T original, T test)
        {
            Assert.LessThanOrEqualTo(original, test);
        }

        public static void ShouldHaveAtLeast<T>(this IEnumerable<T> original, int floor)
        {
            Assert.IsTrue(original.Count() >= floor);
        }

        public static void ShouldHaveNoMoreThan<T>(this IEnumerable<T> original, int ceiling)
        {
            Assert.IsTrue(original.Count() <= ceiling);
        }

        public static void ShouldBeBetween<T>(this T original, T floor, T ceiling)
        {
            Assert.Between(original, floor, ceiling);
        }

        public static void ShouldContain<T>(this IEnumerable<T> original, T item)
        {
            Assert.Contains(original, item);
        }
    }
}