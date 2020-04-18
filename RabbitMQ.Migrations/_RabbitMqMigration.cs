using Newtonsoft.Json;
using RabbitMQ.Migrations.Helpers;
using RabbitMQ.Migrations.Operations;
using System;
using System.Collections.Generic;

namespace RabbitMQ.Migrations
{
    public abstract class RabbitMqMigration
    {
        private IList<BaseOperation> _upOperations;
        private IList<BaseOperation> _downOperations;

        [JsonProperty]
        public IList<BaseOperation> UpOperations => _upOperations ??= BuildOperations(Up);
        [JsonProperty]
        public IList<BaseOperation> DownOperations => _downOperations ??= BuildOperations(Down);

        protected abstract void Up(RabbitMqMigrationBuilder migrationBuilder);
        protected abstract void Down(RabbitMqMigrationBuilder migrationBuilder);

        private static IList<BaseOperation> BuildOperations(Action<RabbitMqMigrationBuilder> buildAction)
        {
            var migrationBuilder = new RabbitMqMigrationBuilder();
            buildAction(migrationBuilder);

            return migrationBuilder.Operations;
        }

        internal string CalculateHash()
        {
            return HashHelper.ComputeSha256Hash(this);
        }
    }
}
