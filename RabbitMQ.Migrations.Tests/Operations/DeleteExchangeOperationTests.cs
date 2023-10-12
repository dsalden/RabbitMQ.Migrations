using AddUp.RabbitMQ.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Migrations.Operations;
using System.Linq;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class DeleteExchangeOperationTests
    {
        [TestMethod]
        public void PropertySettersMinimalTest()
        {
            var operation = new DeleteExchangeOperation()
                .SetName("bar");

            Assert.AreEqual("bar", operation.Name);
            Assert.IsFalse(operation.IfUnused);
        }

        [TestMethod]
        public void PropertySettersFullTest()
        {
            var operation = new DeleteExchangeOperation()
                .SetName("bar")
                .SetIfUnused(true);

            Assert.AreEqual("bar", operation.Name);
            Assert.IsTrue(operation.IfUnused);
        }

        [TestMethod]
        public void ExecuteTest()
        {
            var addOperation = new AddExchangeOperation()
                .SetName("bar")
                .SetType(ExchangeType.Topic);
            var delOperation = new DeleteExchangeOperation()
                .SetName("bar");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                addOperation.Execute(connection, string.Empty);
                delOperation.Execute(connection, string.Empty);
            }

            Assert.AreEqual(0, server.Exchanges.Count(x => !string.IsNullOrEmpty(x.Value.Name)));
        }
    }
}
