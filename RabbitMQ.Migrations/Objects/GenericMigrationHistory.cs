namespace RabbitMQ.Migrations.Objects
{
    internal class GenericMigrationHistory
    {
        public GenericMigrationHistory()
        {
            Version = 1;
        }

        public int Version { get; set; }
    }
}
