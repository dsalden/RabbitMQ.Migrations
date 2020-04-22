using RabbitMQ.Migrations.Operations;
using System.Collections.Generic;

namespace RabbitMQ.Migrations.Objects.v2
{
    internal class MigrationHistoryRowDetails
    {
        public MigrationHistoryRowDetails()
        {
            DownOperations = new List<BaseOperation>();
        }

        public string Name { get; set; }

        public string Hash { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<BaseOperation> DownOperations { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
