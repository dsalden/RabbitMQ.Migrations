using System.Collections.Generic;

namespace RabbitMQ.Migrations.Objects.v2
{
    internal class MigrationHistory
    {
        public MigrationHistory()
        {
            AllMigrations = new List<MigrationHistoryRow>();
            Version = 2;
        }

        public int Version { get; set; }

        public IList<MigrationHistoryRow> AllMigrations { get; }
    }
}
