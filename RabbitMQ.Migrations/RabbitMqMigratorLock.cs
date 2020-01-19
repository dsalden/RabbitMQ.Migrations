using System;
using RabbitMQ.Client;

namespace RabbitMQ.Migrations
{
    internal class RabbitMqMigratorLock : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _model;

        public RabbitMqMigratorLock(IConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _model.QueueDeclare(Constants.LockQueue, false, true, true, null);
        }

        public void Dispose()
        {
            _model?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
