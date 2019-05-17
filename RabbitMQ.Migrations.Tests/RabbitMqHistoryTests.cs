using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using RabbitMQ.Fakes.models;
using RabbitMQ.Migrations.Objects;

namespace RabbitMQ.Migrations.Tests
{
    [TestClass]
    public class RabbitMqHistoryTests
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
            Assert.AreEqual(0, result.AllMigrations.Count);
        }

        [TestMethod]
        public void TestGetAllAppliedMigrationsEmptyQueue()
        {
            _rabbitMqHistory.Init();
            var result = _rabbitMqHistory.GetAllAppliedMigrations();

            Assert.IsNotNull(result);
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
            Assert.AreEqual(1, result.AllMigrations.Count);
            var migration = result.AllMigrations.First();
            Assert.AreEqual("test", migration.Prefix);
            Assert.AreEqual(1, migration.AppliedMigrations.Count);
            Assert.AreEqual("001_TestMigration", migration.AppliedMigrations.First());
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
            Assert.AreEqual("001_TestMigration", result.AppliedMigrations.First());
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

            var mirgationHistoryRow = new MigrationHistoryRow { Prefix = "test" };
            mirgationHistoryRow.AppliedMigrations.Add("001_TestMigration");

            _rabbitMqHistory.UpdateAppliedMigrations(mirgationHistoryRow);

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var message = channel.BasicGet(Constants.HistoryQueue, true);
                Assert.IsNotNull(message);

                var migrationHistory = JsonConvert.DeserializeObject<MigrationHistory>(Encoding.UTF8.GetString(message.Body));
                Assert.IsNotNull(migrationHistory);
                Assert.AreEqual(1, migrationHistory.AllMigrations.Count);
                var migration = migrationHistory.AllMigrations.First();
                Assert.AreEqual("test", migration.Prefix);
                Assert.AreEqual(1, migration.AppliedMigrations.Count);
                Assert.AreEqual("001_TestMigration", migration.AppliedMigrations.First());
            }
        }

        [TestMethod]
        public void TestUpdateAppliedMigrationsExistingPrefix()
        {
            _rabbitMqHistory.Init();
            SetupDefaultExchange();

            PushDummyMigrationHistoryMessage();
            var mirgationHistoryRow = new MigrationHistoryRow { Prefix = "test" };
            mirgationHistoryRow.AppliedMigrations.Add("001_TestMigration");
            mirgationHistoryRow.AppliedMigrations.Add("002_TestMigrationAddQueue");

            _rabbitMqHistory.UpdateAppliedMigrations(mirgationHistoryRow);

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var message = channel.BasicGet(Constants.HistoryQueue, true);
                Assert.IsNotNull(message);

                var migrationHistory = JsonConvert.DeserializeObject<MigrationHistory>(Encoding.UTF8.GetString(message.Body));
                Assert.IsNotNull(migrationHistory);
                Assert.AreEqual(1, migrationHistory.AllMigrations.Count);
                var migration = migrationHistory.AllMigrations.First();
                Assert.AreEqual("test", migration.Prefix);
                Assert.AreEqual(2, migration.AppliedMigrations.Count);
                CollectionAssert.AreEqual(mirgationHistoryRow.AppliedMigrations.ToList(), migration.AppliedMigrations.ToList());
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
            var mirgationHistoryRow = new MigrationHistoryRow { Prefix = "test" };
            mirgationHistoryRow.AppliedMigrations.Add("001_TestMigration");
            var migrationHistory = new MigrationHistory();
            migrationHistory.AllMigrations.Add(mirgationHistoryRow);

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(migrationHistory));
                channel.BasicPublish("", Constants.HistoryQueue, false, null, messageBody);
            }
        }
    }
}
