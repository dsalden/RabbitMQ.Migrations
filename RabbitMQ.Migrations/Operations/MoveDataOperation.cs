using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.Migrations.Operations
{
    public class MoveDataOperation : BaseOperation
    {
        internal string SourceQueueName;
        internal string DestinationQueueName;

        internal override void Execute(IModel model, string prefix)
        {
            BasicGetResult message;
            while ((message = model.BasicGet(SourceQueueName, false)) != null)
            {
                model.BasicPublish(Constants.DefaultExchange, DestinationQueueName, message.BasicProperties, message.Body);
                //model.BasicAck(message.DeliveryTag, false);
            }
        }

        internal MoveDataOperation SetSourceQueueName(string value)
        {
            SourceQueueName = value;
            return this;
        }

        internal MoveDataOperation SetDestinationQueueName(string value)
        {
            DestinationQueueName = value;
            return this;
        }
    }
}
