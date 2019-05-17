using System.Collections.Generic;

namespace RabbitMQ.Migrations.Objects
{
    public class MigrationHistoryRow
    {
        public MigrationHistoryRow()
        {
            AppliedMigrations = new List<string>();
        }

        public string Prefix { get; set; }
        public IList<string> AppliedMigrations { get; set; }
    }
}
