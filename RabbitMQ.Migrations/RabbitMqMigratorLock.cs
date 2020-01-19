using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Threading;

namespace RabbitMQ.Migrations
{
    internal class RabbitMqMigratorLock : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _model;
        private int _counter;

        public RabbitMqMigratorLock(IConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            StartLock();
        }

        public void Dispose()
        {
            _model?.Close();
            _model?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }

        private void StartLock()
        {
            try
            {
                _model.QueueDeclare(Constants.LockQueue, false, true, true, null);
            }
            catch (OperationInterruptedException)
            {
                _counter++;
                if (_counter < 3)
                {
                    Thread.Sleep(1000);
                    StartLock();
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
