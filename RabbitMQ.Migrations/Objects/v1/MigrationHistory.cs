using System.Collections.Generic;

namespace RabbitMQ.Migrations.Objects.v1
{
    internal class MigrationHistory
    {
        public MigrationHistory()
        {
            AllMigrations = new List<MigrationHistoryRow>();
        }

        public IList<MigrationHistoryRow> AllMigrations { get; }
    }
}
