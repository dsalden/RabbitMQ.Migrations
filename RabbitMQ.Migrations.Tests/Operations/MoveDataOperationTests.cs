using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using RabbitMQ.Migrations.Operations;
using System.Linq;
using System.Text;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class MoveDataOperationTests
    {
        [TestMethod]
        public void PropertySettersMinimalTest()
        {
            var operation = new MoveDataOperation()
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
            var moveDataOperation = new MoveDataOperation()
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

                    channel.BasicPublish("", "bar", false, null, Encoding.UTF8.GetBytes("message1"));
                    channel.BasicPublish("", "bar", false, null, Encoding.UTF8.GetBytes("message2"));
                    channel.BasicPublish("", "bar", false, null, Encoding.UTF8.GetBytes("message3"));
                    channel.BasicPublish("", "bar", false, null, Encoding.UTF8.GetBytes("message4"));
                    channel.BasicPublish("", "bar", false, null, Encoding.UTF8.GetBytes("message5"));
                }

                Assert.AreEqual(5, server.Queues.Values.First(x => x.Name == "bar").Messages.Count);
                Assert.AreEqual(0, server.Queues.Values.First(x => x.Name == "foo").Messages.Count);

                moveDataOperation.Execute(connection, string.Empty);

                Assert.AreEqual(0, server.Queues.Values.First(x => x.Name == "bar").Messages.Count);
                Assert.AreEqual(5, server.Queues.Values.First(x => x.Name == "foo").Messages.Count);
            }
        }
    }
}
