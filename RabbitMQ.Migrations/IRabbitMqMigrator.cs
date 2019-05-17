namespace RabbitMQ.Migrations
{
    public interface IRabbitMqMigrator
    {
        void UpdateModel(string prefix);
        void RevertAll(string prefix);
    }
}