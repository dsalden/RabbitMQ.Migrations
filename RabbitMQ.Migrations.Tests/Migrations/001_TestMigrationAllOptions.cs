using RabbitMQ.Client;
using RabbitMQ.Migrations.Attributes;

namespace RabbitMQ.Migrations.Tests.Migrations
{
    [RabbitMqMigration("001_TestMigrationAllOptions", "AllOptionsTest")]
    public class TestMigrationAllOptions : RabbitMqMigration
    {
        protected override void Up(RabbitMqMigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddTopicExchange("fooTopic").SetDurable(true);
            migrationBuilder.AddDirectExchange("fooDirect").SetDurable(true);
            migrationBuilder.AddFanoutExchange("fooFan").SetDurable(true);
            migrationBuilder.AddHeadersExchange("fooHead").SetDurable(true);
            migrationBuilder.AddExchange("fooDel", ExchangeType.Topic);

            migrationBuilder.AddQueue("bar");
            migrationBuilder.AddQueue("fooQueue");
            migrationBuilder.AddQueue("barDel");

            migrationBuilder.BindExchange("fooTopic", "fooQueue", "#").AddArgument("foo", "bar");
            migrationBuilder.BindQueue("bar", "fooFan", "#").AddArgument("foo", "bar");

            migrationBuilder.MoveDataToQueue("barDel", "bar");
            migrationBuilder.MoveDataToExchange("bar", "fooTopic");

            migrationBuilder.DeleteQueue("barDel").SetIfEmpty(true);
            migrationBuilder.DeleteExchange("fooDel").SetIfUnused(true);
        }

        protected override void Down(RabbitMqMigrationBuilder migrationBuilder)
        {
        }
    }
}
