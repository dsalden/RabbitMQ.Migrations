using RabbitMQ.Migrations.Objects;

namespace RabbitMQ.Migrations
{
    public interface IRabbitMqHistory
    {
        void Init();
        MigrationHistory GetAllAppliedMigrations();
        MigrationHistoryRow GetAppliedMigrations(string prefix);
        void UpdateAppliedMigrations(MigrationHistoryRow appliedMigration);
    }
}
