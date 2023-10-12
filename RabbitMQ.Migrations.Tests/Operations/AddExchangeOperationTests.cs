using AddUp.RabbitMQ.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Migrations.Operations;
using System.Linq;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class AddExchangeOperationTests
    {
        [TestMethod]
        public void PropertySettersMinimalTest()
        {
            var operation = new AddExchangeOperation()
                .SetName("bar")
                .SetType(ExchangeType.Topic);

            Assert.AreEqual("bar", operation.Name);
            Assert.AreEqual(ExchangeType.Topic, operation.Type);
            Assert.IsFalse(operation.AutoDelete);
            Assert.IsFalse(operation.Durable);
            Assert.AreEqual(0, operation.Arguments.Count);
            Assert.AreEqual(0, operation.BindExchangeOperations.Count);
        }

        [TestMethod]
        public void PropertySettersFullTest()
        {
            var operation = new AddExchangeOperation()
                .SetName("bar")
                .SetType(ExchangeType.Topic)
                .SetAutoDelete(true)
                .SetDurable(true)
                .AddArgument("foo", "foo-bar")
                .AddExchangeBind("foo", "#");

            Assert.AreEqual("bar", operation.Name);
            Assert.AreEqual(ExchangeType.Topic, operation.Type);
            Assert.IsTrue(operation.AutoDelete);
            Assert.IsTrue(operation.Durable);
            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("foo"));
            Assert.AreEqual("foo-bar", operation.Arguments["foo"]);
            Assert.AreEqual(1, operation.BindExchangeOperations.Count);
            var subOperation = operation.BindExchangeOperations.First();
            Assert.AreEqual("bar", subOperation.DestinationExchangeName);
            Assert.AreEqual("foo", subOperation.SourceExchangeName);
            Assert.AreEqual("#", subOperation.RoutingKey);
        }

        [TestMethod]
        public void ExecuteMinimalTest()
        {
            var operation = new AddExchangeOperation()
                .SetName("bar")
                .SetType(ExchangeType.Topic);

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                operation.Execute(connection, string.Empty);
            }

            Assert.AreEqual(1, server.Exchanges.Count(x => !string.IsNullOrEmpty(x.Value.Name)));
            var exchange = server.Exchanges.Values.First(x => !string.IsNullOrEmpty(x.Name));
            Assert.AreEqual("bar", exchange.Name);
            Assert.AreEqual(ExchangeType.Topic, exchange.Type);
            Assert.IsFalse(exchange.AutoDelete);
            Assert.IsFalse(exchange.IsDurable);
            Assert.AreEqual(0, exchange.Arguments.Count);
            Assert.AreEqual(0, exchange.Bindings.Count);
            //Exchange-to-exchange bindings not supported in fakes...
        }

        [TestMethod]
        public void ExecuteFullTest()
        {
            var operation = new AddExchangeOperation()
                .SetName("bar")
                .SetType(ExchangeType.Topic)
                .SetAutoDelete(true)
                .SetDurable(true)
                .AddArgument("foo", "foo-bar")
                .AddExchangeBind("foo", "#");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using var connection = connectionFactory.CreateConnection();
            operation.Execute(connection, string.Empty);

            Assert.AreEqual(1, server.Exchanges.Count(x => !string.IsNullOrEmpty(x.Value.Name)));
            var exchange = server.Exchanges.Values.First(x => !string.IsNullOrEmpty(x.Name));
            Assert.AreEqual("bar", exchange.Name);
            Assert.AreEqual(ExchangeType.Topic, exchange.Type);
            Assert.IsTrue(exchange.AutoDelete);
            Assert.IsTrue(exchange.IsDurable);
            Assert.AreEqual(1, exchange.Arguments.Count);
            Assert.IsTrue(exchange.Arguments.ContainsKey("foo"));
            Assert.AreEqual("foo-bar", exchange.Arguments["foo"]);
            Assert.AreEqual(0, exchange.Bindings.Count);
            //Exchange-to-exchange bindings not supported in fakes...
        }
    }
}
