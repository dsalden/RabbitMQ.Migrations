using RabbitMQ.Client;

namespace RabbitMQ.Migrations.Operations
{
    public class MoveDataToQueueOperation : BaseOperation
    {
        private const string _originalRoutingKey = "ORG_RoutingKey";
        internal string SourceQueueName;
        internal string DestinationQueueName;

        internal override void Execute(IModel model, string prefix)
        {
            BasicGetResult message;
            while ((message = model.BasicGet(SourceQueueName, false)) != null)
            {
                // Save the original routingKey in the props header
                var props = message.BasicProperties;
                props.Headers.Add(_originalRoutingKey, message.RoutingKey);

                model.BasicPublish(Constants.DefaultExchange, DestinationQueueName, props, message.Body);
                model.BasicAck(message.DeliveryTag, false);
            }
        }

        internal MoveDataToQueueOperation SetSourceQueueName(string value)
        {
            SourceQueueName = value;
            return this;
        }

        internal MoveDataToQueueOperation SetDestinationQueueName(string value)
        {
            DestinationQueueName = value;
            return this;
        }
    }
}
