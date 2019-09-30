namespace RabbitMQ.Migrations
{
    public interface IRabbitMqMigrator
    {
        void UpdateModel(string prefix = null);
        void RevertAll(string prefix = null);
    }
}