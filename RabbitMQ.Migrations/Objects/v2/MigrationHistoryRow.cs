using System.Collections.Generic;

namespace RabbitMQ.Migrations.Objects.v2
{
    internal class MigrationHistoryRow
    {
        public MigrationHistoryRow()
        {
            AppliedMigrations = new List<MigrationHistoryRowDetails>();
        }

        public string Prefix { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<MigrationHistoryRowDetails> AppliedMigrations { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
