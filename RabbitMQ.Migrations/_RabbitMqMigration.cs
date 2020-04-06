using RabbitMQ.Migrations.Extensions;
using RabbitMQ.Migrations.Operations;
using System;
using System.Collections.Generic;

namespace RabbitMQ.Migrations
{
    public abstract class RabbitMqMigration
    {
        private IList<BaseOperation> _upOperations;
        private IList<BaseOperation> _downOperations;

        public IList<BaseOperation> UpOperations => _upOperations ?? (_upOperations = BuildOperations(Up));
        public IList<BaseOperation> DownOperations => _downOperations ?? (_downOperations = BuildOperations(Down));

        protected abstract void Up(RabbitMqMigrationBuilder migrationBuilder);
        protected abstract void Down(RabbitMqMigrationBuilder migrationBuilder);

        private static IList<BaseOperation> BuildOperations(Action<RabbitMqMigrationBuilder> buildAction)
        {
            var migrationBuilder = new RabbitMqMigrationBuilder();
            buildAction(migrationBuilder);

            return migrationBuilder.Operations;
        }

        internal int CalculateHash()
        {
            var hashCode = new HashCode();
            UpOperations.ForEach(x => hashCode.Add(x));
            DownOperations.ForEach(x => hashCode.Add(x));
            return hashCode.ToHashCode();
        }
    }
}
