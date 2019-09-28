using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using RabbitMQ.Migrations.Operations;
using System.Linq;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class AddQueueOperationTests
    {
        [TestMethod]
        public void PropertySettersMinimalTest()
        {
            var operation = new AddQueueOperation()
                .SetName("bar");

            Assert.AreEqual("bar", operation.Name);
            Assert.IsFalse(operation.AutoDelete);
            Assert.IsFalse(operation.Durable);
            Assert.IsFalse(operation.Exclusive);
            Assert.AreEqual(0, operation.Arguments.Count);
            Assert.AreEqual(0, operation.BindQueueOperations.Count);
        }

        [TestMethod]
        public void PropertySettersFullTest()
        {
            var operation = new AddQueueOperation()
                .SetName("bar")
                .SetAutoDelete()
                .SetDurable(true)
                .SetExclusive()
                .AddArgument("foo", "foo-bar")
                .AddQueueBind("foo", "#");

            Assert.AreEqual("bar", operation.Name);
            Assert.IsTrue(operation.AutoDelete);
            Assert.IsTrue(operation.Durable);
            Assert.IsTrue(operation.Exclusive);
            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("foo"));
            Assert.AreEqual("foo-bar", operation.Arguments["foo"]);
            Assert.AreEqual(1, operation.BindQueueOperations.Count);
            var subOperation = operation.BindQueueOperations.First();
            Assert.AreEqual("bar", subOperation.QueueName);
            Assert.AreEqual("foo", subOperation.ExchangeName);
            Assert.AreEqual("#", subOperation.RoutingKey);
        }

        [TestMethod]
        public void ExecuteMinimalTest()
        {
            var operation = new AddQueueOperation()
                .SetName("bar");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                operation.Execute(connection, string.Empty);
            }

            Assert.AreEqual(1, server.Queues.Count);
            var queue = server.Queues.Values.First();
            Assert.AreEqual("bar", queue.Name);
            Assert.IsFalse(queue.IsAutoDelete);
            Assert.IsFalse(queue.IsDurable);
            Assert.IsFalse(queue.IsExclusive);
            Assert.AreEqual(0, queue.Arguments.Count);
            Assert.AreEqual(0, queue.Bindings.Count);
        }

        [TestMethod]
        public void ExecuteFullTest()
        {
            var exchangeOperation = new AddExchangeOperation()
                .SetName("foo")
                .SetType(ExchangeType.Topic);

            var operation = new AddQueueOperation()
                .SetName("bar")
                .SetAutoDelete()
                .SetDurable(true)
                .SetExclusive()
                .AddArgument("foo", "foo-bar")
                .AddQueueBind("foo", "#");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                exchangeOperation.Execute(connection, string.Empty);
                operation.Execute(connection, string.Empty);
            }

            Assert.AreEqual(1, server.Queues.Count);
            var queue = server.Queues.Values.First();
            Assert.AreEqual("bar", queue.Name);
            Assert.IsTrue(queue.IsAutoDelete);
            Assert.IsTrue(queue.IsDurable);
            Assert.IsTrue(queue.IsExclusive);
            Assert.AreEqual(1, queue.Arguments.Count);
            Assert.IsTrue(queue.Arguments.ContainsKey("foo"));
            Assert.AreEqual("foo-bar", queue.Arguments["foo"]);
            Assert.AreEqual(1, queue.Bindings.Count);
            var binding = queue.Bindings.First();
            Assert.AreEqual("foo", binding.Value.Exchange.Name);
            Assert.AreEqual("bar", binding.Value.Queue.Name);
            Assert.AreEqual("#", binding.Value.RoutingKey);
        }
    }
}
