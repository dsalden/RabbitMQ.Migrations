using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Fakes;
using RabbitMQ.Migrations.Operations;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class DeleteQueueOperationTests
    {
        [TestMethod]
        public void PropertySettersMinimalTest()
        {
            var operation = new DeleteQueueOperation()
                .SetName("bar");

            Assert.AreEqual("bar", operation.Name);
            Assert.IsFalse(operation.IfEmpty);
            Assert.IsFalse(operation.IfUnused);
        }

        [TestMethod]
        public void PropertySettersFullTest()
        {
            var operation = new DeleteQueueOperation()
                .SetName("bar")
                .SetIfEmpty(true)
                .SetIfUnused(true);

            Assert.AreEqual("bar", operation.Name);
            Assert.IsTrue(operation.IfEmpty);
            Assert.IsTrue(operation.IfUnused);
        }

        [TestMethod]
        public void ExecuteTest()
        {
            var addOperation = new AddQueueOperation()
                .SetName("bar");
            var delOperation = new DeleteQueueOperation()
                .SetName("bar");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                addOperation.Execute(connection, string.Empty);
                delOperation.Execute(connection, string.Empty);
            }

            Assert.AreEqual(0, server.Queues.Count);
        }
    }
}
