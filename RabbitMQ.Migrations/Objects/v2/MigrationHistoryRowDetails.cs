using System.Collections.Generic;
using RabbitMQ.Migrations.Operations;

namespace RabbitMQ.Migrations.Objects.v2
{
    public class MigrationHistoryRowDetails
    {
        public MigrationHistoryRowDetails()
        {
            DownOperations = new List<BaseOperation>();
        }

        public string Name { get; set; }

        public int Hash { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<BaseOperation> DownOperations { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
