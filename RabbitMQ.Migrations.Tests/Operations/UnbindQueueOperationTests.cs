using AddUp.RabbitMQ.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Migrations.Operations;
using System.Linq;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class UnbindQueueOperationTests
    {
        [TestMethod]
        public void PropertySettersMinimalTest()
        {
            var operation = new UnbindQueueOperation()
                .SetExchangeName("foo")
                .SetQueueName("bar")
                .SetRoutingKey("#");

            Assert.AreEqual("bar", operation.QueueName);
            Assert.AreEqual("foo", operation.ExchangeName);
            Assert.AreEqual("#", operation.RoutingKey);
            Assert.AreEqual(0, operation.Arguments.Count);
        }

        [TestMethod]
        public void PropertySettersFullTest()
        {
            var operation = new UnbindQueueOperation()
                .SetExchangeName("foo")
                .SetQueueName("bar")
                .SetRoutingKey("#")
                .AddArgument("foo", "foo-bar");

            Assert.AreEqual("bar", operation.QueueName);
            Assert.AreEqual("foo", operation.ExchangeName);
            Assert.AreEqual("#", operation.RoutingKey);
            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("foo"));
            Assert.AreEqual("foo-bar", operation.Arguments["foo"]);
        }

        [TestMethod]
        public void ExecuteFullTest()
        {
            var operation = new UnbindQueueOperation()
                .SetExchangeName("foo")
                .SetQueueName("bar")
                .SetRoutingKey("#");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    model.ExchangeDeclare("foo", ExchangeType.Topic, true);
                    model.QueueDeclare("bar", true);
                    model.QueueBind("bar", "foo", "#", null);
                }

                operation.Execute(connection, string.Empty);
            }

            Assert.AreEqual(1, server.Queues.Count);
            var queue = server.Queues.Values.First();
            Assert.AreEqual("bar", queue.Name);
            Assert.AreEqual(1, queue.Bindings.Count);
        }
    }
}
