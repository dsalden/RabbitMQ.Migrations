using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using RabbitMQ.Migrations.Operations;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class BindExchangeOperationTests
    {
        [TestMethod]
        public void PropertySettersMinimalTest()
        {
            var operation = new BindExchangeOperation()
                .SetSourceExchangeName("bar")
                .SetDestinationExchangeName("foo")
                .SetRoutingKey("#");

            Assert.AreEqual("bar", operation.SourceExchangeName);
            Assert.AreEqual("foo", operation.DestinationExchangeName);
            Assert.AreEqual("#", operation.RoutingKey);
            Assert.AreEqual(0, operation.Arguments.Count);
        }

        [TestMethod]
        public void PropertySettersFullTest()
        {
            var operation = new BindExchangeOperation()
                .SetSourceExchangeName("bar")
                .SetDestinationExchangeName("foo")
                .SetRoutingKey("#")
                .AddArgument("foo", "foo-bar");

            Assert.AreEqual("bar", operation.SourceExchangeName);
            Assert.AreEqual("foo", operation.DestinationExchangeName);
            Assert.AreEqual("#", operation.RoutingKey);
            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("foo"));
            Assert.AreEqual("foo-bar", operation.Arguments["foo"]);
        }

        [TestMethod]
        public void ExecuteFullTest()
        {
            var operation = new BindExchangeOperation()
                .SetSourceExchangeName("foo")
                .SetDestinationExchangeName("bar")
                .SetRoutingKey("#")
                .AddArgument("foo", "foo-bar");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    model.ExchangeDeclare("foo", ExchangeType.Topic, true);
                    model.QueueDeclare("bar", true);
                }

                operation.Execute(connection, string.Empty);
            }

            Assert.AreEqual(1, server.Queues.Count);
            Assert.AreEqual(1, server.Exchanges.Count);
            //Exchange-to-exchange bindings not supported in fakes...
            //Arguments on bindings not supported in fakes...
        }
    }
}
