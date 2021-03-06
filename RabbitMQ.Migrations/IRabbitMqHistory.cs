﻿using RabbitMQ.Migrations.Objects.v2;

namespace RabbitMQ.Migrations
{
    internal interface IRabbitMqHistory
    {
        void Init();
        MigrationHistory GetAllAppliedMigrations();
        MigrationHistoryRow GetAppliedMigrations(string prefix);
        void UpdateAppliedMigrations(MigrationHistoryRow appliedMigration);
    }
}
