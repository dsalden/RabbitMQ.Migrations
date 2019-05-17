using RabbitMQ.Migrations.Attributes;
using RabbitMQ.Migrations.Extensions;

namespace RabbitMQ.Migrations.Demo.Migrations
{
    [RabbitMqMigration("002_AddAcdrQueue")]
    public class AddAcdrQueue : RabbitMqMigration
    {
        private const string PolarisExchangeIn = "PolarisExchangeIn";
        private const string PolarisDeadLetterExchangeIn = "PolarisDeadLetterExchangeIn";

        private const string PolarisAcdrQueueIn = "PolarisAcdrQueueIn";

        protected override void Up(RabbitMqMigrationBuilder migrationBuilder)
        {
            //setup inbound queues
            migrationBuilder.AddQueue(PolarisAcdrQueueIn)
                .SetDurable(true)
                .SetAutoDelete(false)
                .SetDeadLetterExchange(PolarisDeadLetterExchangeIn)
                .AddQueueBind(PolarisExchangeIn, "urn.un.unece.uncefact.fisheries.*.ACDR.EU.2")
                .AddQueueBind(PolarisExchangeIn, "urn.un.unece.uncefact.fisheries.*.ACDR.EU.2.#");
        }

        protected override void Down(RabbitMqMigrationBuilder migrationBuilder)
        {
            //delete queues
            migrationBuilder.DeleteQueue(PolarisAcdrQueueIn);
        }
    }
}