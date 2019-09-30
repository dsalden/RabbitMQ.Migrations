using RabbitMQ.Client;
using RabbitMQ.Migrations.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RabbitMQ.Migrations
{
    public class RabbitMqMigrator : IRabbitMqMigrator
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IRabbitMqHistory _rabbitMqHistory;

        public RabbitMqMigrator(IConnectionFactory connectionFactory, IRabbitMqHistory rabbitMqHistory)
        {
            Guard.ArgumentNotNull(nameof(rabbitMqHistory), rabbitMqHistory);

            _connectionFactory = connectionFactory;
            _rabbitMqHistory = rabbitMqHistory;

            _rabbitMqHistory.Init();
        }

        public void UpdateModel(string prefix = null)
        {
            //Find all unapplied migrations and apply using up operations
            //Update model with all unapplied migrations
            var appliedMigrations = _rabbitMqHistory.GetAppliedMigrations(prefix);

            var allMigrations = GetAllRabbitMqMigrations(prefix);
            using (var connection = _connectionFactory.CreateConnection())
            {
                foreach (var migrationInfo in allMigrations.Where(x => appliedMigrations.AppliedMigrations.All(y => x.Key != y)).OrderBy(x => x.Key))
                {
                    var migration = CreateRabbitMqMigration(migrationInfo.Value);
                    foreach (var operation in migration.UpOperations)
                    {
                        operation.Execute(connection, prefix);
                    }

                    appliedMigrations.AppliedMigrations.Add(migrationInfo.Key);
                }

                _rabbitMqHistory.UpdateAppliedMigrations(appliedMigrations);
            }
        }

        public void RevertAll(string prefix = null)
        {
            //Find all applied migrations and rollback using down operations
            //Update model with all unapplied migrations
            var appliedMigrations = _rabbitMqHistory.GetAppliedMigrations(prefix);

            var allMigrations = GetAllRabbitMqMigrations(prefix);
            using (var connection = _connectionFactory.CreateConnection())
            {
                foreach (var migrationInfo in allMigrations.Where(x => appliedMigrations.AppliedMigrations.Any(y => x.Key == y)).OrderByDescending(x => x.Key))
                {
                    var migration = CreateRabbitMqMigration(migrationInfo.Value);
                    foreach (var operation in migration.DownOperations)
                    {
                        operation.Execute(connection, prefix);
                    }

                    appliedMigrations.AppliedMigrations.Remove(migrationInfo.Key);
                }

                _rabbitMqHistory.UpdateAppliedMigrations(appliedMigrations);
            }
        }

        private static IReadOnlyDictionary<string, TypeInfo> GetAllRabbitMqMigrations(string prefix)
        {
            return (from a in AppDomain.CurrentDomain.GetAssemblies()
                    from type in a.DefinedTypes
                    where type.IsSubclassOf(typeof(RabbitMqMigration))
                          && (string.IsNullOrEmpty(prefix) || type.GetCustomAttribute<RabbitMqMigrationAttribute>()?.Prefix == prefix)
                    let id = type.GetCustomAttribute<RabbitMqMigrationAttribute>()?.Id
                    orderby id
                    select (id, type))
                .ToDictionary(tuple => tuple.id, tuple => tuple.type);
        }

        private static RabbitMqMigration CreateRabbitMqMigration(TypeInfo migrationClass)
        {
            var migration = (RabbitMqMigration)Activator.CreateInstance(migrationClass.AsType());

            return migration;
        }
    }
}
