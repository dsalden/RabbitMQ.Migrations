using RabbitMQ.Migrations.Objects.v2;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ.Migrations.Extensions
{
    internal static class MigrationHistoryExtensions
    {
        public static MigrationHistoryRowDetails GetMigration(this MigrationHistoryRow historyRow, string migrationName)
        {
            Guard.ArgumentNotNull(nameof(historyRow), historyRow);

            return historyRow.AppliedMigrations.FirstOrDefault(x => x.Name == migrationName);
        }

        public static void AddMigration(this MigrationHistoryRow historyRow, KeyValuePair<string, RabbitMqMigration> migration)
        {
            Guard.ArgumentNotNull(nameof(historyRow), historyRow);

            historyRow.AppliedMigrations.Add(new MigrationHistoryRowDetails
            {
                Name = migration.Key,
                Hash = migration.Value?.CalculateHash(),
                DownOperations = migration.Value?.DownOperations
            });
        }

        public static void RemoveMigration(this MigrationHistoryRow historyRow, string migrationName)
        {
            Guard.ArgumentNotNull(nameof(historyRow), historyRow);

            historyRow.AppliedMigrations.Remove(historyRow.AppliedMigrations.First(x => x.Name == migrationName));
        }

        public static void UpdateMigration(this MigrationHistoryRowDetails historyRowDetails, RabbitMqMigration migration)
        {
            Guard.ArgumentNotNull(nameof(historyRowDetails), historyRowDetails);
            Guard.ArgumentNotNull(nameof(migration), migration);

            historyRowDetails.Hash = migration.CalculateHash();
            historyRowDetails.DownOperations = migration.DownOperations;
        }
    }
}
