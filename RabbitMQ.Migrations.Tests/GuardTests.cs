using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Migrations.Tests
{
    namespace GuardTests
    {
        [TestClass]
        public class ArgumentNotNullTests
        {
            [TestMethod]
            public void ShouldNotThrow()
            {
                Guard.ArgumentNotNull("argName", "name");
            }

            [TestMethod]
            public void ShouldThrowOnNull()
            {
                Assert.ThrowsException<ArgumentNullException>(() => Guard.ArgumentNotNull("argName", null));
            }
        }

        [TestClass]
        public class ArgumentNotNullOrEmptyTests
        {
            [TestMethod]
            public void ShouldNotThrow()
            {
                Guard.ArgumentNotNullOrEmpty("argName", "name");
            }

            [TestMethod]
            public void ShouldThrowOnNull()
            {
                Assert.ThrowsException<ArgumentNullException>(() => Guard.ArgumentNotNullOrEmpty("argName", null));
            }

            [TestMethod]
            public void ShouldThrowOnEmpty()
            {
                Assert.ThrowsException<ArgumentException>(() => Guard.ArgumentNotNullOrEmpty("argName", string.Empty));
            }
        }

        [TestClass]
        public class ArgumentNotNullOrEmptyCollectionTests
        {
            [TestMethod]
            public void ShouldNotThrow()
            {
                Guard.ArgumentNotNullOrEmpty("argName", new List<string>{"name"});
            }

            [TestMethod]
            public void ShouldThrowOnNull()
            {
                Assert.ThrowsException<ArgumentNullException>(() => Guard.ArgumentNotNullOrEmpty<string>("argName", null));
            }

            [TestMethod]
            public void ShouldThrowOnEmpty()
            {
                Assert.ThrowsException<ArgumentException>(() => Guard.ArgumentNotNullOrEmpty("argName", new List<string>()));
            }
        }

        [TestClass]
        public class ArgumentValidTests
        {
            [TestMethod]
            public void ShouldNotThrow()
            {
                Guard.ArgumentValid(true, "argName", "message");
            }

            [TestMethod]
            public void ShouldThrow()
            {
                Assert.ThrowsException<ArgumentException>(() => Guard.ArgumentValid(false, "argName", "message"), "message");
            }
        }

        [TestClass]
        public class NotDisposedTests
        {
            [TestMethod]
            public void ShouldNotThrow()
            {
                Guard.NotDisposed(false, "message");
            }

            [TestMethod]
            public void ShouldThrow()
            {
                Assert.ThrowsException<ObjectDisposedException>(() => Guard.NotDisposed(true, "message"), "message");
            }
        }

        [TestClass]
        public class OperationValidTests
        {
            [TestMethod]
            public void ShouldNotThrow()
            {
                Guard.OperationValid(true, "message");
            }

            [TestMethod]
            public void ShouldThrow()
            {
                Assert.ThrowsException<InvalidOperationException>(() => Guard.OperationValid(false, "message"), "message");
            }
        }
    }
}
