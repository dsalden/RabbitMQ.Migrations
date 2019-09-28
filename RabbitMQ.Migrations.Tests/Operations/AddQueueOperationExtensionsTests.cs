using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Fakes;
using RabbitMQ.Migrations.Extensions;
using RabbitMQ.Migrations.Operations;
using System.Linq;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class AddQueueOperationExtensionsTests
    {
        private static AddQueueOperation GetBasicAddQueueOperation()
        {
            return new AddQueueOperation()
                .SetName("bar");
        }

        [TestMethod]
        public void SetMessageTimeToLiveTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetMessageTimeToLive(5000);

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("x-message-ttl"));
            Assert.AreEqual(5000, operation.Arguments["x-message-ttl"]);
        }

        [TestMethod]
        public void SetAutoExpireTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetAutoExpire(15000);

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("x-expires"));
            Assert.AreEqual(15000, operation.Arguments["x-expires"]);
        }

        [TestMethod]
        public void SetMaxLengthTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetMaxLength(10000);

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("x-max-length"));
            Assert.AreEqual(10000, operation.Arguments["x-max-length"]);
        }

        [TestMethod]
        public void SetMaxLengthBytesTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetMaxLengthBytes(10000);

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("x-max-length-bytes"));
            Assert.AreEqual(10000, operation.Arguments["x-max-length-bytes"]);
        }

        [TestMethod]
        public void SetOverflowBehaviourTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetOverflowBehaviour("drop-head");

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("x-overflow"));
            Assert.AreEqual("drop-head", operation.Arguments["x-overflow"]);
        }

        [TestMethod]
        public void SetDeadLetterExchangeTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetDeadLetterExchange("foo");

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("x-dead-letter-exchange"));
            Assert.AreEqual("foo", operation.Arguments["x-dead-letter-exchange"]);
        }

        [TestMethod]
        public void SetDeadLetterRoutingKeyTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetDeadLetterRoutingKey("bla");

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("x-dead-letter-routing-key"));
            Assert.AreEqual("bla", operation.Arguments["x-dead-letter-routing-key"]);
        }

        [TestMethod]
        public void SetMaxPriorityTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetMaxPriority(10);

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("x-max-priority"));
            Assert.AreEqual(10, operation.Arguments["x-max-priority"]);
        }

        [TestMethod]
        public void SetLazyModeTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetLazyMode();

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("x-queue-mode"));
            Assert.AreEqual("lazy", operation.Arguments["x-queue-mode"]);
        }

        [TestMethod]
        public void SetMasterLocationTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetMasterLocation("server123");

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("x-queue-master-locator"));
            Assert.AreEqual("server123", operation.Arguments["x-queue-master-locator"]);
        }

        [TestMethod]
        public void ExecuteFullTest()
        {
            var operation = GetBasicAddQueueOperation()
                .SetMessageTimeToLive(5000)
                .SetAutoExpire(15000)
                .SetMaxLength(10000)
                .SetMaxLengthBytes(10000)
                .SetOverflowBehaviour("drop-head")
                .SetDeadLetterExchange("foo")
                .SetDeadLetterRoutingKey("bla")
                .SetMaxPriority(10)
                .SetLazyMode()
                .SetMasterLocation("server123");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                operation.Execute(connection, string.Empty);
            }

            Assert.AreEqual(1, server.Queues.Count);
            var queue = server.Queues.Values.First();
            Assert.AreEqual(10, queue.Arguments.Count);
            Assert.IsTrue(queue.Arguments.ContainsKey("x-message-ttl"));
            Assert.AreEqual(5000, queue.Arguments["x-message-ttl"]);
            Assert.IsTrue(queue.Arguments.ContainsKey("x-expires"));
            Assert.AreEqual(15000, queue.Arguments["x-expires"]);
            Assert.IsTrue(queue.Arguments.ContainsKey("x-max-length"));
            Assert.AreEqual(10000, queue.Arguments["x-max-length"]);
            Assert.IsTrue(queue.Arguments.ContainsKey("x-max-length-bytes"));
            Assert.AreEqual(10000, queue.Arguments["x-max-length-bytes"]);
            Assert.IsTrue(queue.Arguments.ContainsKey("x-overflow"));
            Assert.AreEqual("drop-head", queue.Arguments["x-overflow"]);
            Assert.IsTrue(queue.Arguments.ContainsKey("x-dead-letter-exchange"));
            Assert.AreEqual("foo", queue.Arguments["x-dead-letter-exchange"]);
            Assert.IsTrue(queue.Arguments.ContainsKey("x-dead-letter-routing-key"));
            Assert.AreEqual("bla", queue.Arguments["x-dead-letter-routing-key"]);
            Assert.IsTrue(queue.Arguments.ContainsKey("x-max-priority"));
            Assert.AreEqual(10, queue.Arguments["x-max-priority"]);
            Assert.IsTrue(queue.Arguments.ContainsKey("x-queue-mode"));
            Assert.AreEqual("lazy", queue.Arguments["x-queue-mode"]);
            Assert.IsTrue(queue.Arguments.ContainsKey("x-queue-master-locator"));
            Assert.AreEqual("server123", queue.Arguments["x-queue-master-locator"]);
        }
    }
}
