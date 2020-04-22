using RabbitMQ.Migrations.Extensions;
using System.Linq;

namespace RabbitMQ.Migrations.Helpers
{
    internal static class MigrationHistoryUpgradeHelper
    {
        public static Objects.v2.MigrationHistory UpgradeToV2(Objects.v1.MigrationHistory migrationHistoryV1)
        {
            if (migrationHistoryV1 == null) return null;

            var migrationHistoryV2 = new Objects.v2.MigrationHistory();
            migrationHistoryV2.AllMigrations.AddRange(migrationHistoryV1.AllMigrations.Select(x => new Objects.v2.MigrationHistoryRow
            {
                Prefix = x.Prefix,
                AppliedMigrations = x.AppliedMigrations.Select(y => new Objects.v2.MigrationHistoryRowDetails { Name = y }).ToList()
            }));

            return migrationHistoryV2;
        }
    }
}
