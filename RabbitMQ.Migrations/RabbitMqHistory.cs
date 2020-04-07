using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Migrations.Exceptions;
using RabbitMQ.Migrations.Helpers;
using RabbitMQ.Migrations.Objects;
using RabbitMQ.Migrations.Objects.v2;
using System.Linq;
using System.Text;

namespace RabbitMQ.Migrations
{
    internal class RabbitMqHistory : IRabbitMqHistory
    {
        private readonly IConnectionFactory _connectionFactory;
        internal static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.Auto
        };

        public RabbitMqHistory(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Init()
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var model = connection.CreateModel())
            {
                model.QueueDeclare(Constants.HistoryQueue, true, false, false);
            }
        }

        public MigrationHistory GetAllAppliedMigrations()
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var model = connection.CreateModel())
            {
                var result = model.BasicGet(Constants.HistoryQueue, false);
                if (result == null) return new MigrationHistory();

                model.BasicNack(result.DeliveryTag, false, true);
                var migrationHistoryJson = Encoding.UTF8.GetString(result.Body);
                return EnsureLatestMigrationHistoryVersion(migrationHistoryJson);
            }
        }

        public MigrationHistoryRow GetAppliedMigrations(string prefix)
        {
            var allAppliedMigrations = GetAllAppliedMigrations();

            return allAppliedMigrations.AllMigrations.Any(x => x.Prefix == prefix)
                ? allAppliedMigrations.AllMigrations.FirstOrDefault(x => x.Prefix == prefix)
                : new MigrationHistoryRow { Prefix = prefix };
        }

        public void UpdateAppliedMigrations(MigrationHistoryRow appliedMigration)
        {
            Guard.ArgumentNotNull(nameof(appliedMigration), appliedMigration);

            var allAppliedMigrations = GetAllAppliedMigrations();
            if (allAppliedMigrations.AllMigrations.Any(x => x.Prefix == appliedMigration.Prefix))
            {
                var currentMigrations = allAppliedMigrations.AllMigrations.First(x => x.Prefix == appliedMigration.Prefix);

                currentMigrations.AppliedMigrations = appliedMigration.AppliedMigrations;
            }
            else
            {
                allAppliedMigrations.AllMigrations.Add(appliedMigration);
            }

            using (var connection = _connectionFactory.CreateConnection())
            using (var model = connection.CreateModel())
            {
                model.BasicGet(Constants.HistoryQueue, true);

                var messageText = JsonConvert.SerializeObject(allAppliedMigrations, JsonSerializerSettings);
                var messageProps = model.CreateBasicProperties();
                messageProps.Persistent = true;
                model.BasicPublish(Constants.DefaultExchange, Constants.HistoryQueue, messageProps, Encoding.UTF8.GetBytes(messageText));
            }
        }

        private static MigrationHistory EnsureLatestMigrationHistoryVersion(string migrationHistoryJson)
        {
            var migrationHistory = JsonConvert.DeserializeObject<GenericMigrationHistory>(migrationHistoryJson, JsonSerializerSettings);
            switch (migrationHistory.Version)
            {
                case 1:
                    // v1 uses the default JSON Serializer settings
                    var migrationHistoryV1 = JsonConvert.DeserializeObject<Objects.v1.MigrationHistory>(migrationHistoryJson);
                    return MigrationHistoryUpgradeHelper.UpgradeToV2(migrationHistoryV1);
                case 2:
                    return JsonConvert.DeserializeObject<MigrationHistory>(migrationHistoryJson, JsonSerializerSettings);
                default:
                    throw new RabbitMqMigrationException($"Invalid RabbitMq History version {migrationHistory.Version}");
            }
        }
    }
}
