using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using RabbitMQ.Migrations.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class MoveDataToExchangeOperationTests
    {
        [TestMethod]
        public void PropertySettersMinimalTest()
        {
            var operation = new MoveDataToExchangeOperation()
                .SetSourceQueueName("bar")
                .SetDestinationExchangeName("foo");

            Assert.AreEqual("bar", operation.SourceQueueName);
            Assert.AreEqual("foo", operation.DestinationExchangeName);
        }

        [TestMethod]
        public void ExecuteTest()
        {
            var addBarQueueOperation = new AddQueueOperation()
                .SetName("bar");
            var addFooExchangeOperation = new AddExchangeOperation()
                .SetName("foo");
            var addTstQueueOperation = new AddQueueOperation()
                .SetName("tst")
                .AddQueueBind("foo", "bar");
            var moveDataOperation = new MoveDataToExchangeOperation()
                .SetSourceQueueName("bar")
                .SetDestinationExchangeName("foo");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                addBarQueueOperation.Execute(connection, string.Empty);
                addFooExchangeOperation.Execute(connection, string.Empty);
                addTstQueueOperation.Execute(connection, string.Empty);

                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("", ExchangeType.Direct, true);
                    channel.QueueBind("bar", "", "bar", null);

                    for (int i = 0; i < 10; i++)
                    {
                        var props = channel.CreateBasicProperties();
                        props.Headers = new Dictionary<string, object>();

                        channel.BasicPublish("", "bar", false, props, Encoding.UTF8.GetBytes($"message{i}"));
                    }
                }

                Assert.AreEqual(10, server.Queues.Values.First(x => x.Name == "bar").Messages.Count);
                Assert.AreEqual(0, server.Queues.Values.First(x => x.Name == "tst").Messages.Count);

                moveDataOperation.Execute(connection, string.Empty);

                Assert.AreEqual(0, server.Queues.Values.First(x => x.Name == "bar").Messages.Count);
                Assert.AreEqual(10, server.Queues.Values.First(x => x.Name == "tst").Messages.Count);
            }
        }
    }
}
