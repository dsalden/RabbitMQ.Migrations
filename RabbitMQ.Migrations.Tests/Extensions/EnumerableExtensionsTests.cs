using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Migrations.Extensions;
using System;

namespace RabbitMQ.Migrations.Tests.Extensions
{
    [TestClass]
    public class EnumerableExtensionsTests
    {
        [TestMethod]
        public void ForEachTest()
        {
            var array = new[] {"foo", "bar"};

            var count = 0;
            array.ForEach(x => count++);

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ForEachNullTest()
        {
            var count = 0;
            EnumerableExtensions.ForEach<string>(null, x => count++);
        }
    }
}
