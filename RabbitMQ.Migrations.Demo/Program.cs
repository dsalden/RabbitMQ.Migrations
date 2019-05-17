using RabbitMQ.Client;

namespace RabbitMQ.Migrations.Demo
{
    public static class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory { HostName = "192.168.0.16" };

            IRabbitMqMigrator migrator = new RabbitMqMigrator(factory, new RabbitMqHistory(factory));
            migrator.UpdateModel(null);
            //migrator.RevertAll(factory, null);
        }
    }
}
