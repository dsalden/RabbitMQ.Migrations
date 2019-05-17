using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using RabbitMQ.Migrations.Extensions;
using RabbitMQ.Migrations.Operations;

namespace RabbitMQ.Migrations.Tests.Operations
{
    [TestClass]
    public class AddExchangeOperationExtensionsTests
    {
        private static AddExchangeOperation GetBasicAddExchangeOperation()
        {
            return new AddExchangeOperation()
                .SetName("bar")
                .SetType(ExchangeType.Topic);
        }

        [TestMethod]
        public void SetAlternativeExchangeTest()
        {
            var operation = GetBasicAddExchangeOperation()
                .SetAlternativeExchange("foo");

            Assert.AreEqual(1, operation.Arguments.Count);
            Assert.IsTrue(operation.Arguments.ContainsKey("alternate-exchange"));
            Assert.AreEqual("foo", operation.Arguments["alternate-exchange"]);
        }

        [TestMethod]
        public void ExecuteFullTest()
        {
            var operation = GetBasicAddExchangeOperation()
                .SetAlternativeExchange("foo");

            var server = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(server);
            using (var connection = connectionFactory.CreateConnection())
            {
                operation.Execute(connection, string.Empty);
            }

            Assert.AreEqual(1, server.Exchanges.Count);
            var exchange = server.Exchanges.Values.First();
            Assert.AreEqual(1, exchange.Arguments.Count);
            Assert.IsTrue(exchange.Arguments.Contains("alternate-exchange"));
            Assert.AreEqual("foo", exchange.Arguments["alternate-exchange"]);
        }
    }
}
