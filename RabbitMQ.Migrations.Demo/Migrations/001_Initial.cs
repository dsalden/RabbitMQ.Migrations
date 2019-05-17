using RabbitMQ.Migrations.Attributes;
using RabbitMQ.Migrations.Extensions;

namespace RabbitMQ.Migrations.Demo.Migrations
{
    [RabbitMqMigration("001_Initial")]
    public class Initial : RabbitMqMigration
    {
        private const string PolarisExchangeIn = "PolarisExchangeIn";
        private const string PolarisExchangeOut = "PolarisExchangeOut";
        private const string PolarisDeadLetterExchangeIn = "PolarisDeadLetterExchangeIn";
        private const string PolarisDeadLetterExchangeOut = "PolarisDeadLetterExchangeOut";

        private const string PolarisAssignedOnQueueIn = "PolarisAssignedONQueueIn";
        private const string PolarisFishingActivityQueueIn = "PolarisFishingActivityQueueIn";
        private const string PolarisMdrQueueIn = "PolarisMdrQueueIn";
        private const string PolarisSalesQueueIn = "PolarisSalesQueueIn";
        private const string PolarisVesselQueueIn = "PolarisVesselQueueIn";

        private const string PolarisAuctionQueueOut = "PolarisAuctionQueueOut";
        private const string PolarisFluxQueueOut = "PolarisFluxQueueOut";
        private const string PolarisMCatchQueueOut = "PolarisMCatchQueueOut";

        private const string PolarisDeadLetterQueueIn = "PolarisDeadLetterQueueIn";
        private const string PolarisDeadLetterQueueOut = "PolarisDeadLetterQueueOut";

        protected override void Up(RabbitMqMigrationBuilder migrationBuilder)
        {
            //setup dead letter exchanges
            migrationBuilder.AddTopicExchange(PolarisDeadLetterExchangeIn)
                .SetDurable(true);
            migrationBuilder.AddTopicExchange(PolarisDeadLetterExchangeOut)
                .SetDurable(true);

            //setup topic exchanges
            migrationBuilder.AddTopicExchange(PolarisExchangeIn)
                .SetDurable(true)
                .SetAlternativeExchange(PolarisDeadLetterExchangeIn);
            migrationBuilder.AddTopicExchange(PolarisExchangeOut)
                .SetDurable(true)
                .SetAlternativeExchange(PolarisDeadLetterExchangeOut);

            //setup inbound queues
            migrationBuilder.AddQueue(PolarisSalesQueueIn)
                .SetDurable(true)
                .SetAutoDelete(false)
                .SetDeadLetterExchange(PolarisDeadLetterExchangeIn)
                .AddQueueBind(PolarisExchangeIn, "urn.un.unece.uncefact.fisheries.*.SALES.EU.2")
                .AddQueueBind(PolarisExchangeIn, "urn.un.unece.uncefact.fisheries.*.SALES.EU.2.#");

            migrationBuilder.AddQueue(PolarisVesselQueueIn)
                .SetDurable(true)
                .SetAutoDelete(false)
                .SetDeadLetterExchange(PolarisDeadLetterExchangeIn)
                .AddQueueBind(PolarisExchangeIn, "urn.un.unece.uncefact.fisheries.*.VESSEL.EU.2")
                .AddQueueBind(PolarisExchangeIn, "urn.un.unece.uncefact.fisheries.*.VESSEL.EU.2.#");

            migrationBuilder.AddQueue(PolarisMdrQueueIn)
                .SetDurable(true)
                .SetAutoDelete(false)
                .SetDeadLetterExchange(PolarisDeadLetterExchangeIn)
                .AddQueueBind(PolarisExchangeIn, "urn.un.unece.uncefact.fisheries.*.MDM.EU.2")
                .AddQueueBind(PolarisExchangeIn, "urn.un.unece.uncefact.fisheries.*.MDM.EU.2.#");

            migrationBuilder.AddQueue(PolarisFishingActivityQueueIn)
                .SetDurable(true)
                .SetAutoDelete(false)
                .SetDeadLetterExchange(PolarisDeadLetterExchangeIn)
                .AddQueueBind(PolarisExchangeIn, "urn.un.unece.uncefact.fisheries.*.FA.EU.2")
                .AddQueueBind(PolarisExchangeIn, "urn.un.unece.uncefact.fisheries.*.FA.EU.2.#");

            migrationBuilder.AddQueue(PolarisAssignedOnQueueIn)
                .SetDurable(true)
                .SetAutoDelete(false)
                .SetDeadLetterExchange(PolarisDeadLetterExchangeIn)
                .AddQueueBind(PolarisExchangeIn, "FLUX.ON");

            //setup outbound queues
            migrationBuilder.AddQueue(PolarisFluxQueueOut)
                .SetDurable(true)
                .SetAutoDelete(false)
                .SetDeadLetterExchange(PolarisDeadLetterExchangeOut)
                .AddQueueBind(PolarisExchangeOut, "urn.un.unece.uncefact.fisheries.FLUX.#");

            migrationBuilder.AddQueue(PolarisAuctionQueueOut)
                .SetDurable(true)
                .SetAutoDelete(false)
                .SetDeadLetterExchange(PolarisDeadLetterExchangeOut)
                .AddQueueBind(PolarisExchangeOut, "urn.un.unece.uncefact.fisheries.AUCTION.#");

            migrationBuilder.AddQueue(PolarisMCatchQueueOut)
                .SetDurable(true)
                .SetAutoDelete(false)
                .SetDeadLetterExchange(PolarisDeadLetterExchangeOut)
                .AddQueueBind(PolarisExchangeOut, "urn.un.unece.uncefact.fisheries.MCATCH.#");

            //setup dead letter queues
            migrationBuilder.AddQueue(PolarisDeadLetterQueueIn)
                .SetDurable(true)
                .SetAutoDelete(false)
                .AddQueueBind(PolarisDeadLetterExchangeIn, "#");

            migrationBuilder.AddQueue(PolarisDeadLetterQueueOut)
                .SetDurable(true)
                .SetAutoDelete(false)
                .AddQueueBind(PolarisDeadLetterExchangeOut, "#");
        }

        protected override void Down(RabbitMqMigrationBuilder migrationBuilder)
        {
            //delete queues
            migrationBuilder.DeleteQueue(PolarisAssignedOnQueueIn);
            migrationBuilder.DeleteQueue(PolarisFishingActivityQueueIn);
            migrationBuilder.DeleteQueue(PolarisMdrQueueIn);
            migrationBuilder.DeleteQueue(PolarisSalesQueueIn);
            migrationBuilder.DeleteQueue(PolarisVesselQueueIn);

            migrationBuilder.DeleteQueue(PolarisAuctionQueueOut);
            migrationBuilder.DeleteQueue(PolarisFluxQueueOut);
            migrationBuilder.DeleteQueue(PolarisMCatchQueueOut);

            migrationBuilder.DeleteQueue(PolarisDeadLetterQueueIn);
            migrationBuilder.DeleteQueue(PolarisDeadLetterQueueOut);

            //delete exchanges
            migrationBuilder.DeleteExchange(PolarisDeadLetterExchangeIn);
            migrationBuilder.DeleteExchange(PolarisDeadLetterExchangeOut);
            migrationBuilder.DeleteExchange(PolarisExchangeIn);
            migrationBuilder.DeleteExchange(PolarisExchangeOut);
        }
    }
}
