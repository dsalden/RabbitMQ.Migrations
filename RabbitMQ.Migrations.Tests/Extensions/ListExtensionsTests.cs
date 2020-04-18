using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Migrations.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RabbitMQ.Migrations.Tests.Extensions
{
    [TestClass]
    public class ListExtensionsTests
    {
        [TestMethod]
        public void AddRangeListTest()
        {
            var list1 = new[] {"foo", "bar"};
            IList<string> list2 = new List<string> {"foobar"};

            list2.AddRange(list1);

            Assert.AreEqual(3, list2.Count);
            Assert.IsTrue(list2.Any(x => x == "foobar"));
            Assert.IsTrue(list2.Any(x => x == "foo"));
            Assert.IsTrue(list2.Any(x => x == "bar"));
        }

        [TestMethod]
        public void AddRangeEnumerableTest()
        {
            var list1 = new[] {"foo", "bar"};
            IList<string> list2 = new Collection<string> {"foobar"};

            list2.AddRange(list1);

            Assert.AreEqual(3, list2.Count);
            Assert.IsTrue(list2.Any(x => x == "foobar"));
            Assert.IsTrue(list2.Any(x => x == "foo"));
            Assert.IsTrue(list2.Any(x => x == "bar"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddRangeNullTest1()
        {
            ListExtensions.AddRange(null, new List<string> {"foobar"});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddRangeNullTest2()
        {
            ListExtensions.AddRange(new List<string> {"foobar"}, null);
        }

    }
}
