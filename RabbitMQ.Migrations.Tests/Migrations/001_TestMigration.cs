using RabbitMQ.Migrations.Attributes;

namespace RabbitMQ.Migrations.Tests.Migrations
{
    [RabbitMqMigration("001_TestMigration", "UnitTest")]
    public class TestMigration : RabbitMqMigration
    {
        protected override void Up(RabbitMqMigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddTopicExchange("foo")
                .SetDurable(true);

            migrationBuilder.AddQueue("bar")
                .SetDurable(true)
                .SetAutoDelete(false)
                .AddQueueBind("foo", "#");
        }

        protected override void Down(RabbitMqMigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteQueue("bar");
            migrationBuilder.DeleteExchange("foo");
        }
    }
}
