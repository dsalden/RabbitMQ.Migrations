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

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<string> AppliedMigrations { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
