using System;
using System.Runtime.Serialization;

namespace RabbitMQ.Migrations.Exceptions
{
    [Serializable]
    public class RabbitMqMigrationException : Exception
    {
        public RabbitMqMigrationException()
        {
        }

        public RabbitMqMigrationException(string message)
            : base(message)
        {
        }

        public RabbitMqMigrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client. Nodig voor Domain.BasisArchitectuur.Framework.MVC.BarController.GetAuditData()
        protected RabbitMqMigrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
