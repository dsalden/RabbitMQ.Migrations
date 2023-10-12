using AddUp.RabbitMQ.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Migrations.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class MoveDataToQueueOperationTests
    {
        [TestMethod]
        public void PropertySettersMinimalTest()
        {
            var operation = new MoveDataToQueueOperation()
                .SetSourceQueueName("bar")
                .SetDestinationQueueName("foo");

            Assert.AreEqual("bar", operation.SourceQueueName);
            Assert.AreEqual("foo", operation.DestinationQueueName);
        }

        [TestMethod]
        public void ExecuteTest()
        {
            var addBarQueueOperation = new AddQueueOperation()
                .SetName("bar");
            var addFooQueueOperation = new AddQueueOperation()
                .SetName("foo");
            var moveDataOperation = new MoveDataToQueueOperation()
                .SetSourceQueueName("bar")
                .SetDestinationQueueName("foo");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                addBarQueueOperation.Execute(connection, string.Empty);
                addFooQueueOperation.Execute(connection, string.Empty);

                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("", ExchangeType.Direct, true);
                    channel.QueueBind("bar", "", "bar", null);
                    channel.QueueBind("foo", "", "foo", null);

                    for (int i = 0; i < 10; i++)
                    {
                        var props = channel.CreateBasicProperties();
                        props.Headers = new Dictionary<string, object>();

                        channel.BasicPublish("", "bar", false, props, Encoding.UTF8.GetBytes($"message{i}"));
                    }
                }

                Assert.AreEqual(10, server.Queues.Values.First(x => x.Name == "bar").Messages.Count);
                Assert.AreEqual(0, server.Queues.Values.First(x => x.Name == "foo").Messages.Count);

                moveDataOperation.Execute(connection, string.Empty);

                Assert.AreEqual(0, server.Queues.Values.First(x => x.Name == "bar").Messages.Count);
                Assert.AreEqual(10, server.Queues.Values.First(x => x.Name == "foo").Messages.Count);
            }
        }
    }
}
