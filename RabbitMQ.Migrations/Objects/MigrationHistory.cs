using System.Collections.Generic;

namespace RabbitMQ.Migrations.Objects
{
    public class MigrationHistory
    {
        public MigrationHistory()
        {
            AllMigrations = new List<MigrationHistoryRow>();
        }

        public IList<MigrationHistoryRow> AllMigrations { get; }
    }
}
