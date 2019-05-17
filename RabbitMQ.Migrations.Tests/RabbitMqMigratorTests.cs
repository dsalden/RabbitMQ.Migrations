using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using RabbitMQ.Migrations.Objects;

namespace RabbitMQ.Migrations.Tests
{
    [TestClass]
    public class RabbitMqMigratorTests
    {
        [TestMethod]
        public void TestMigration()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            var history = new Mock<IRabbitMqHistory>();
            history.Setup(x => x.GetAppliedMigrations(It.IsAny<string>())).Returns<string>(prefix => new MigrationHistoryRow {Prefix = prefix});

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.UpdateModel("UnitTest");

            Assert.AreEqual(1, rabbitServer.Exchanges.Count);
            Assert.AreEqual(1, rabbitServer.Queues.Count);
        }

        [TestMethod]
        public void TestMigrationAllReadyUp2Date()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            var history = new Mock<IRabbitMqHistory>();
            history.Setup(x => x.GetAppliedMigrations(It.IsAny<string>())).Returns<string>(prefix =>
                new MigrationHistoryRow { Prefix = prefix, AppliedMigrations = new List<string> { "001_TestMigration" } });

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.UpdateModel("UnitTest");

            Assert.AreEqual(0, rabbitServer.Exchanges.Count);
            Assert.AreEqual(0, rabbitServer.Queues.Count);
        }


        [TestMethod]
        public void TestMigrationAllOptions()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            var history = new Mock<IRabbitMqHistory>();
            history.Setup(x => x.GetAppliedMigrations(It.IsAny<string>())).Returns<string>(prefix => new MigrationHistoryRow { Prefix = prefix });

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.UpdateModel("AllOptionsTest");

            Assert.AreEqual(4, rabbitServer.Exchanges.Count);
            Assert.AreEqual(2, rabbitServer.Queues.Count);
        }

        [TestMethod]
        public void TestRevertMigrations()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);
            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("UnitTest.foo", ExchangeType.Direct, true);
                channel.QueueDeclare("UnitTest.bar", true);
            }

            var history = new Mock<IRabbitMqHistory>();
            history.Setup(x => x.GetAppliedMigrations(It.IsAny<string>())).Returns<string>(prefix =>
                new MigrationHistoryRow {Prefix = prefix, AppliedMigrations = new List<string> {"001_TestMigration"}});

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.RevertAll("UnitTest");

            Assert.AreEqual(0, rabbitServer.Exchanges.Count);
            Assert.AreEqual(0, rabbitServer.Queues.Count);
        }
    }
}
