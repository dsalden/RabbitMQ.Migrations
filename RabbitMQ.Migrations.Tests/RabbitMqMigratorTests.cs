using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using RabbitMQ.Migrations.Objects.v2;
using RabbitMQ.Migrations.Operations;
using System.Collections.Generic;
using System.Linq;

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
            history.Setup(x => x.GetAppliedMigrations(It.IsAny<string>())).Returns<string>(prefix => new MigrationHistoryRow { Prefix = prefix });
            MigrationHistoryRow result = null;
            history.Setup(x => x.UpdateAppliedMigrations(It.IsAny<MigrationHistoryRow>()))
                .Callback<MigrationHistoryRow>(x => result = x);

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.UpdateModel("UnitTest");

            Assert.AreEqual(1, rabbitServer.Exchanges.Count);
            Assert.AreEqual(1, rabbitServer.Queues.Count);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AppliedMigrations.Count);
            Assert.AreEqual("001_TestMigration", result.AppliedMigrations.First().Name);
            Assert.IsNotNull(result.AppliedMigrations.First().Hash);
        }

        [TestMethod]
        public void TestMigrationAllReadyUp2DateNoHash()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            var history = new Mock<IRabbitMqHistory>();
            history.Setup(x => x.GetAppliedMigrations(It.IsAny<string>())).Returns<string>(prefix =>
                new MigrationHistoryRow { Prefix = prefix, AppliedMigrations = new List<MigrationHistoryRowDetails> { new MigrationHistoryRowDetails { Name = "001_TestMigration" } } });
            MigrationHistoryRow result = null;
            history.Setup(x => x.UpdateAppliedMigrations(It.IsAny<MigrationHistoryRow>()))
                .Callback<MigrationHistoryRow>(x => result = x);

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.UpdateModel("UnitTest");

            Assert.AreEqual(0, rabbitServer.Exchanges.Count);
            Assert.AreEqual(0, rabbitServer.Queues.Count);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AppliedMigrations.Count);
            Assert.AreEqual("001_TestMigration", result.AppliedMigrations.First().Name);
            Assert.IsNotNull(result.AppliedMigrations.First().Hash);
        }

        [TestMethod]
        public void TestMigrationAllReadyUp2DateSameHash()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            var history = new Mock<IRabbitMqHistory>();
            history.Setup(x => x.GetAppliedMigrations(It.IsAny<string>())).Returns<string>(prefix =>
                new MigrationHistoryRow
                {
                    Prefix = prefix,
                    AppliedMigrations = new List<MigrationHistoryRowDetails>
                    {
                        new MigrationHistoryRowDetails
                        {
                            Name = "001_TestMigration",
                            Hash = "a6964237ffad49ed9492dca9318f9d793c64aa6d6c7645b7c0db0db9f68d3658"
                        }
                    }
                });
            MigrationHistoryRow result = null;
            history.Setup(x => x.UpdateAppliedMigrations(It.IsAny<MigrationHistoryRow>()))
                .Callback<MigrationHistoryRow>(x => result = x);

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.UpdateModel("UnitTest");

            Assert.AreEqual(0, rabbitServer.Exchanges.Count);
            Assert.AreEqual(0, rabbitServer.Queues.Count);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AppliedMigrations.Count);
            Assert.AreEqual("001_TestMigration", result.AppliedMigrations.First().Name);
            Assert.IsNotNull(result.AppliedMigrations.First().Hash);
            Assert.AreEqual("a6964237ffad49ed9492dca9318f9d793c64aa6d6c7645b7c0db0db9f68d3658", result.AppliedMigrations.First().Hash);
        }

        [TestMethod]
        public void TestMigrationAllReadyUp2DateDifferentHash()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            var history = new Mock<IRabbitMqHistory>();
            history.Setup(x => x.GetAppliedMigrations(It.IsAny<string>())).Returns<string>(prefix =>
                new MigrationHistoryRow
                {
                    Prefix = prefix,
                    AppliedMigrations = new List<MigrationHistoryRowDetails>
                    {
                        new MigrationHistoryRowDetails
                        {
                            Name = "001_TestMigration",
                            Hash = "bla"
                        }
                    }
                });
            MigrationHistoryRow result = null;
            history.Setup(x => x.UpdateAppliedMigrations(It.IsAny<MigrationHistoryRow>()))
                .Callback<MigrationHistoryRow>(x => result = x);

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.UpdateModel("UnitTest");

            Assert.AreEqual(1, rabbitServer.Exchanges.Count);
            Assert.AreEqual(1, rabbitServer.Queues.Count);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AppliedMigrations.Count);
            Assert.AreEqual("001_TestMigration", result.AppliedMigrations.First().Name);
            Assert.IsNotNull(result.AppliedMigrations.First().Hash);
            Assert.AreNotEqual("bla", result.AppliedMigrations.First().Hash);
        }

        [TestMethod]
        public void TestMigrationAllOptions()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            var history = new Mock<IRabbitMqHistory>();
            history.Setup(x => x.GetAppliedMigrations(It.IsAny<string>())).Returns<string>(prefix => new MigrationHistoryRow { Prefix = prefix });
            MigrationHistoryRow result = null;
            history.Setup(x => x.UpdateAppliedMigrations(It.IsAny<MigrationHistoryRow>()))
                .Callback<MigrationHistoryRow>(x => result = x);

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.UpdateModel("AllOptionsTest");

            Assert.AreEqual(4, rabbitServer.Exchanges.Count);
            Assert.AreEqual(2, rabbitServer.Queues.Count);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AppliedMigrations.Count);
            Assert.AreEqual("001_TestMigrationAllOptions", result.AppliedMigrations.First().Name);
            Assert.IsNotNull(result.AppliedMigrations.First().Hash);
        }

        [TestMethod]
        public void TestMigrationMigrationNotPresentAnymore()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);
            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("UnitTest.new_foo", ExchangeType.Direct, true);
                channel.QueueDeclare("UnitTest.new_bar", true);
            }

            var history = new Mock<IRabbitMqHistory>();
            history.Setup(x => x.GetAppliedMigrations(It.IsAny<string>())).Returns<string>(prefix =>
                new MigrationHistoryRow
                {
                    Prefix = prefix,
                    AppliedMigrations = new List<MigrationHistoryRowDetails>
                    {
                        new MigrationHistoryRowDetails { Name = "001_TestMigration" },
                        new MigrationHistoryRowDetails
                        {
                            Name = "002_TestMigration",
                            Hash = "a6964237ffad49ed9492dca9318f9d793c64aa6d6c7645b7c0db0db9f68d3659",
                            DownOperations = new List<BaseOperation>
                            {
                                new DeleteQueueOperation().SetName("new_bar"),
                                new DeleteExchangeOperation().SetName("new_foo")
                            }
                        }
                    }
                });
            MigrationHistoryRow result = null;
            history.Setup(x => x.UpdateAppliedMigrations(It.IsAny<MigrationHistoryRow>()))
                .Callback<MigrationHistoryRow>(x => result = x);

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.UpdateModel("UnitTest");

            Assert.AreEqual(0, rabbitServer.Exchanges.Count);
            Assert.AreEqual(0, rabbitServer.Queues.Count);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AppliedMigrations.Count);
            Assert.AreEqual("001_TestMigration", result.AppliedMigrations.First().Name);
            Assert.IsNotNull(result.AppliedMigrations.First().Hash);
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
                new MigrationHistoryRow
                {
                    Prefix = prefix,
                    AppliedMigrations = new List<MigrationHistoryRowDetails> { new MigrationHistoryRowDetails { Name = "001_TestMigration" } }
                });
            MigrationHistoryRow result = null;
            history.Setup(x => x.UpdateAppliedMigrations(It.IsAny<MigrationHistoryRow>()))
                .Callback<MigrationHistoryRow>(x => result = x);

            var migrator = new RabbitMqMigrator(connectionFactory, history.Object);
            migrator.RevertAll("UnitTest");

            Assert.AreEqual(0, rabbitServer.Exchanges.Count);
            Assert.AreEqual(0, rabbitServer.Queues.Count);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.AppliedMigrations.Count);
        }
    }
}
