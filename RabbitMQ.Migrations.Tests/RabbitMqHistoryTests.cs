using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using AddUp.RabbitMQ.Fakes;
using RabbitMQ.Migrations.Exceptions;
using RabbitMQ.Migrations.Helpers;
using RabbitMQ.Migrations.Objects.v2;

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
        public void EnsureLatestMigrationHistoryVersionWithInvalidVersion()
        {
            _rabbitMqHistory.Init();
            SetupDefaultExchange();

            PushDummyMigrationHistoryMessage();
            Assert.ThrowsException<RabbitMqMigrationException>(() => _rabbitMqHistory.GetAllAppliedMigrations(), "Invalid RabbitMq History version 3");
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
            var migrationHistoryRow = new MigrationHistoryRow {Prefix = "test"};
            var migrationHistory = new MigrationHistory {Version = 3};
            migrationHistory.AllMigrations.Add(migrationHistoryRow);

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var messageBody = JsonConvertHelper.SerializeObjectToByteArray(migrationHistory);
                channel.BasicPublish("", Constants.HistoryQueue, false, null, messageBody);
            }
        }
    }
}
