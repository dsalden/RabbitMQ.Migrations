using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQ.Migrations.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqMigrations(this IServiceCollection services)
        {
            services.AddTransient<IRabbitMqMigrator, RabbitMqMigrator>();
            services.AddTransient<IRabbitMqHistory, RabbitMqHistory>();

            return services;
        }
    }
}
