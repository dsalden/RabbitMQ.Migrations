using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using RabbitMQ.Migrations.Helpers;
using RabbitMQ.Migrations.Objects.v2;
using RabbitMQ.Migrations.Operations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ.Migrations.Tests
{
    [TestClass]
    public class RabbitMqHistoryV2Tests
    {
        private RabbitServer _rabbitServer;
        private IConnectionFactory _connectionFactory;
        private IRabbitMqHistory _rabbitMqHistory;

        [TestInitialize]
        public void TestInitialize()
        {
            _rabbitServer = new RabbitServer();
            _connectionFactory = new FakeConnectionFactory(_rabbitServer);
            _rabbitMqHistory = new RabbitMqHistory(_connectionFactory);
        }

        [TestMethod]
        public void TestHistoryInit()
        {
            _rabbitMqHistory.Init();

            Assert.AreEqual(0, _rabbitServer.Exchanges.Count);
            Assert.AreEqual(1, _rabbitServer.Queues.Count);
        }

        [TestMethod]
        public void TestGetAllAppliedMigrationsNoQueue()
        {
            var result = _rabbitMqHistory.GetAllAppliedMigrations();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Version);
            Assert.AreEqual(0, result.AllMigrations.Count);
        }

        [TestMethod]
        public void TestGetAllAppliedMigrationsEmptyQueue()
        {
            _rabbitMqHistory.Init();
            var result = _rabbitMqHistory.GetAllAppliedMigrations();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Version);
            Assert.AreEqual(0, result.AllMigrations.Count);
        }

        [TestMethod]
        public void TestGetAllAppliedMigrationsFilledQueue()
        {
            _rabbitMqHistory.Init();
            SetupDefaultExchange();

            PushDummyMigrationHistoryMessage();
            var result = _rabbitMqHistory.GetAllAppliedMigrations();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Version);
            Assert.AreEqual(1, result.AllMigrations.Count);
            var migration = result.AllMigrations.First();
            Assert.AreEqual("test", migration.Prefix);
            Assert.AreEqual(1, migration.AppliedMigrations.Count);
            Assert.AreEqual("001_TestMigration", migration.AppliedMigrations.First().Name);
        }

        [TestMethod]
        public void TestGetAppliedMigrations()
        {
            _rabbitMqHistory.Init();
            SetupDefaultExchange();

            PushDummyMigrationHistoryMessage();
            var result = _rabbitMqHistory.GetAppliedMigrations("test");

            Assert.IsNotNull(result);
            Assert.AreEqual("test", result.Prefix);
            Assert.AreEqual(1, result.AppliedMigrations.Count);
            Assert.AreEqual("001_TestMigration", result.AppliedMigrations.First().Name);
        }

        [TestMethod]
        public void TestGetAppliedMigrations2()
        {
            _rabbitMqHistory.Init();
            SetupDefaultExchange();

            PushDummyMigrationHistoryMessage();
            var result = _rabbitMqHistory.GetAppliedMigrations("dummy");

            Assert.IsNotNull(result);
            Assert.AreEqual("dummy", result.Prefix);
            Assert.AreEqual(0, result.AppliedMigrations.Count);
        }

        [TestMethod]
        public void TestUpdateAppliedMigrationsNewPrefix()
        {
            _rabbitMqHistory.Init();
            SetupDefaultExchange();

            var migrationHistoryRow = new MigrationHistoryRow { Prefix = "test" };
            migrationHistoryRow.AppliedMigrations.Add(new MigrationHistoryRowDetails { Name = "001_TestMigration" });

            _rabbitMqHistory.UpdateAppliedMigrations(migrationHistoryRow);

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var message = channel.BasicGet(Constants.HistoryQueue, true);
                Assert.IsNotNull(message);

                var migrationHistory = JsonConvertHelper.DeserializeObject<MigrationHistory>(message.Body);
                Assert.IsNotNull(migrationHistory);
                Assert.AreEqual(1, migrationHistory.AllMigrations.Count);
                var migration = migrationHistory.AllMigrations.First();
                Assert.AreEqual("test", migration.Prefix);
                Assert.AreEqual(1, migration.AppliedMigrations.Count);
                Assert.AreEqual("001_TestMigration", migration.AppliedMigrations.First().Name);
            }
        }

        [TestMethod]
        public void TestUpdateAppliedMigrationsExistingPrefix()
        {
            _rabbitMqHistory.Init();
            SetupDefaultExchange();

            PushDummyMigrationHistoryMessage();
            var migrationHistoryRow = new MigrationHistoryRow { Prefix = "test" };
            migrationHistoryRow.AppliedMigrations.Add(new MigrationHistoryRowDetails { Name = "001_TestMigration" });
            migrationHistoryRow.AppliedMigrations.Add(new MigrationHistoryRowDetails { Name = "002_TestMigrationAddQueue" });

            _rabbitMqHistory.UpdateAppliedMigrations(migrationHistoryRow);

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var message = channel.BasicGet(Constants.HistoryQueue, true);
                Assert.IsNotNull(message);

                var migrationHistory = JsonConvertHelper.DeserializeObject<MigrationHistory>(message.Body);
                Assert.IsNotNull(migrationHistory);
                Assert.AreEqual(1, migrationHistory.AllMigrations.Count);
                var migration = migrationHistory.AllMigrations.First();
                Assert.AreEqual("test", migration.Prefix);
                Assert.AreEqual(2, migration.AppliedMigrations.Count);
                CollectionAssert.AreEqual(migrationHistoryRow.AppliedMigrations.ToList(), migration.AppliedMigrations.ToList(), new AppliedMigrationsComparer());
            }
        }

        private void SetupDefaultExchange()
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("", ExchangeType.Direct, true);
                channel.QueueBind(Constants.HistoryQueue, "", Constants.HistoryQueue, null);
            }
        }

        private void PushDummyMigrationHistoryMessage()
        {
            var migrationHistoryRow = new MigrationHistoryRow { Prefix = "test" };
            migrationHistoryRow.AppliedMigrations.Add(new MigrationHistoryRowDetails
            {
                Name = "001_TestMigration",
                Hash = "a0b87bef6d840b00ac344eb2a204442760794512bb8bc0873b63d8c7d5849e9f",
                DownOperations = new List<BaseOperation>
                {
                    new DeleteQueueOperation().SetName("bar"),
                    new DeleteExchangeOperation().SetName("foo")
                }
            });
            var migrationHistory = new MigrationHistory();
            migrationHistory.AllMigrations.Add(migrationHistoryRow);

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var messageBody = JsonConvertHelper.SerializeObjectToByteArray(migrationHistory);
                channel.BasicPublish("", Constants.HistoryQueue, false, null, messageBody);
            }
        }

        public class AppliedMigrationsComparer : IComparer
        {
            int IComparer.Compare(object x, object y)
            {
                if (x is MigrationHistoryRowDetails xTyped && y is MigrationHistoryRowDetails yTyped)
                {
                    return xTyped.Name == yTyped.Name ? 0 : 1;
                }

                return 1;
            }
        }
    }
}
