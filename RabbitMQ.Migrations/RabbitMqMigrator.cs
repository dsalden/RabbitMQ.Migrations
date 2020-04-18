using RabbitMQ.Client;
using RabbitMQ.Migrations.Attributes;
using RabbitMQ.Migrations.Exceptions;
using RabbitMQ.Migrations.Extensions;
using RabbitMQ.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
                {
                    using var connection = _connectionFactory.CreateConnection();
                    foreach (var migrationInfo in allMigrations.OrderBy(x => x.Key))
                    {
                        var applied = appliedMigrations.GetMigration(migrationInfo.Key);
                        if (applied == null)
                        {
                            // Apply migration operations
                            ApplyOperations(connection, prefix, migrationInfo.Value.UpOperations);
                            appliedMigrations.AddMigration(migrationInfo);
                        }
                        else if (!string.IsNullOrEmpty(applied.Hash) && applied.Hash != migrationInfo.Value.CalculateHash())
                        {
                            // Rollback old version of migration
                            ApplyOperations(connection, prefix, applied.DownOperations);
                            // Apply new version of migration
                            ApplyOperations(connection, prefix, migrationInfo.Value.UpOperations);
                            applied.UpdateMigration(migrationInfo.Value);
                        }
                        else if (string.IsNullOrEmpty(applied.Hash))
                        {
                            applied.UpdateMigration(migrationInfo.Value);
                        }
                    }

                    foreach (var applied in appliedMigrations.AppliedMigrations.Where(x => allMigrations.All(y => y.Key != x.Name)).ToList())
                    {
                        // Rollback migration because not present in current application
                        ApplyOperations(connection, prefix, applied.DownOperations);
                        appliedMigrations.RemoveMigration(applied.Name);
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
                throw new RabbitMqMigrationException("Could not update RabbitMQ model", ex);
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
                {
                    using var connection = _connectionFactory.CreateConnection();
                    foreach (var applied in appliedMigrations.AppliedMigrations.ToList())
                    {
                        var downOperations = applied.DownOperations;
                        if (!downOperations.Any() && allMigrations.ContainsKey(applied.Name))
                        {
                            downOperations = allMigrations[applied.Name].DownOperations;
                        }

                        ApplyOperations(connection, prefix, downOperations);
                        appliedMigrations.RemoveMigration(applied.Name);
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
                throw new RabbitMqMigrationException("Could not revert RabbitMQ model", ex);
            }
        }

        private static IReadOnlyDictionary<string, RabbitMqMigration> GetAllRabbitMqMigrations(string prefix)
        {
            return (from a in AppDomain.CurrentDomain.GetAssemblies()
                    from type in a.DefinedTypes
                    where type.IsSubclassOf(typeof(RabbitMqMigration))
                            && (string.IsNullOrEmpty(prefix) || type.GetCustomAttribute<RabbitMqMigrationAttribute>()?.Prefix == prefix)
                    let id = type.GetCustomAttribute<RabbitMqMigrationAttribute>()?.Id
                    orderby id
                    select (id, type))
                .ToDictionary(tuple => tuple.id, tuple => CreateRabbitMqMigration(tuple.type));
        }

        private static RabbitMqMigration CreateRabbitMqMigration(TypeInfo migrationClass)
        {
            return (RabbitMqMigration)Activator.CreateInstance(migrationClass.AsType());
        }

        private static void ApplyOperations(IConnection connection, string prefix, IEnumerable<BaseOperation> operations)
        {
            operations?.ForEach(x => x.Execute(connection, prefix));
        }
    }
}
