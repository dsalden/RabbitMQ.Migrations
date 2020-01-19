using RabbitMQ.Migrations.Operations;
using System;
using System.Collections.Generic;

namespace RabbitMQ.Migrations
{
    public abstract class RabbitMqMigration
    {
        private List<BaseOperation> _upOperations;
        private List<BaseOperation> _downOperations;

        public IEnumerable<BaseOperation> UpOperations => _upOperations ?? (_upOperations = BuildOperations(Up));
        public IEnumerable<BaseOperation> DownOperations => _downOperations ?? (_downOperations = BuildOperations(Down));

        protected abstract void Up(RabbitMqMigrationBuilder migrationBuilder);
        protected abstract void Down(RabbitMqMigrationBuilder migrationBuilder);

        private static List<BaseOperation> BuildOperations(Action<RabbitMqMigrationBuilder> buildAction)
        {
            var migrationBuilder = new RabbitMqMigrationBuilder();
            buildAction(migrationBuilder);

            return migrationBuilder.Operations;
        }
    }
}
