using AddUp.RabbitMQ.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Migrations.Operations;
using System.Linq;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class BindQueueOperationTests
    {
        [TestMethod]
        public void PropertySettersMinimalTest()
        {
            var operation = new BindQueueOperation()
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
            var operation = new BindQueueOperation()
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
            var operation = new BindQueueOperation()
                .SetExchangeName("foo")
                .SetQueueName("bar")
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
            var queue = server.Queues.Values.First();
            Assert.AreEqual("bar", queue.Name);
            Assert.AreEqual(2, queue.Bindings.Count);
            var binding = queue.Bindings.First(x => !string.IsNullOrEmpty(x.Value.Exchange.Name));
            Assert.AreEqual("foo", binding.Value.Exchange.Name);
            Assert.AreEqual("bar", binding.Value.Queue.Name);
            Assert.AreEqual("#", binding.Value.RoutingKey);
            //Arguments on bindings not supported in fakes...
        }
    }
}
