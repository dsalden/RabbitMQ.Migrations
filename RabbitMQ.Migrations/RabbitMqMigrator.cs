using RabbitMQ.Client;
using RabbitMQ.Migrations.Attributes;
using RabbitMQ.Migrations.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RabbitMQ.Migrations.Objects.v2;

namespace RabbitMQ.Migrations
{
    internal class RabbitMqMigrator : IRabbitMqMigrator
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
            try
            {
                //Find all unapplied migrations and apply using up operations
                //Update model with all unapplied migrations
                var appliedMigrations = _rabbitMqHistory.GetAppliedMigrations(prefix);

                var allMigrations = GetAllRabbitMqMigrations(prefix);
                using (new RabbitMqMigratorLock(_connectionFactory))
                using (var connection = _connectionFactory.CreateConnection())
                {
                    foreach (var migrationInfo in allMigrations.Where(x => appliedMigrations.AppliedMigrations.All(y => x.Key != y.Name)).OrderBy(x => x.Key))
                    {
                        var migration = CreateRabbitMqMigration(migrationInfo.Value);
                        foreach (var operation in migration.UpOperations)
                        {
                            operation.Execute(connection, prefix);
                        }

                        appliedMigrations.AppliedMigrations.Add(new MigrationHistoryRowDetails { Name = migrationInfo.Key, Hash = migration.CalculateHash(), DownOperations = migration.DownOperations });
                    }

                    _rabbitMqHistory.UpdateAppliedMigrations(appliedMigrations);
                }
            }
            catch (FileLoadException ex)
            {
                throw new RabbitMqMigrationException($"Could not update RabbitMQ model: could not load file {ex.FileName}", ex);
            }
            catch (Exception ex)
            {
                throw new RabbitMqMigrationException($"Could not update RabbitMQ model", ex);
            }
        }

        public void RevertAll(string prefix = null)
        {
            try
            {
                //Find all applied migrations and rollback using down operations
                //Update model with all unapplied migrations
                var appliedMigrations = _rabbitMqHistory.GetAppliedMigrations(prefix);

                var allMigrations = GetAllRabbitMqMigrations(prefix);
                using (new RabbitMqMigratorLock(_connectionFactory))
                using (var connection = _connectionFactory.CreateConnection())
                {
                    foreach (var migrationInfo in allMigrations.Where(x => appliedMigrations.AppliedMigrations.Any(y => x.Key == y.Name)).OrderByDescending(x => x.Key))
                    {
                        var migration = CreateRabbitMqMigration(migrationInfo.Value);
                        foreach (var operation in migration.DownOperations)
                        {
                            operation.Execute(connection, prefix);
                        }

                        appliedMigrations.AppliedMigrations.Remove(appliedMigrations.AppliedMigrations.First(x => x.Name == migrationInfo.Key));
                    }

                    _rabbitMqHistory.UpdateAppliedMigrations(appliedMigrations);
                }
            }
            catch (FileLoadException ex)
            {
                throw new RabbitMqMigrationException($"Could not revert RabbitMQ model: could not load file {ex.FileName}", ex);
            }
            catch (Exception ex)
            {
                throw new RabbitMqMigrationException($"Could not revert RabbitMQ model", ex);
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
