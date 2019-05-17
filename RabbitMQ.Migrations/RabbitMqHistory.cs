using System.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Migrations.Objects;
using System.Text;

namespace RabbitMQ.Migrations
{
    public class RabbitMqHistory : IRabbitMqHistory
    {
        private readonly IConnectionFactory _connectionFactory;

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
                return JsonConvert.DeserializeObject<MigrationHistory>(Encoding.UTF8.GetString(result.Body));
            }
        }

        public MigrationHistoryRow GetAppliedMigrations(string prefix)
        {
            var allAppliedMigrations = GetAllAppliedMigrations();

            return allAppliedMigrations.AllMigrations.Any(x => x.Prefix == prefix)
                ? allAppliedMigrations.AllMigrations.FirstOrDefault(x => x.Prefix == prefix)
                : new MigrationHistoryRow {Prefix = prefix};
        }

        public void UpdateAppliedMigrations(MigrationHistoryRow appliedMigration)
        {
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

                var messageText = JsonConvert.SerializeObject(allAppliedMigrations);
                var messageProps = model.CreateBasicProperties();
                messageProps.Persistent = true;
                model.BasicPublish("", Constants.HistoryQueue, messageProps, Encoding.UTF8.GetBytes(messageText));
            }
        }
    }
}
