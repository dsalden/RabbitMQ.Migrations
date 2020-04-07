using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Migrations.DependencyInjection;

namespace RabbitMQ.Migrations.Demo
{
    public static class Program
    {
        public static void Main()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConnectionFactory>(new ConnectionFactory { HostName = "192.168.0.16" });
            serviceCollection.AddRabbitMqMigrations();
            var serviceProvider = serviceCollection.BuildServiceProvider(true);

            var migrator = serviceProvider.GetService<IRabbitMqMigrator>();
            migrator.UpdateModel(null);
            //migrator.RevertAll(factory, null);
        }
    }
}
