using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ.Migrations.Operations
{
    public class MoveDataToExchangeOperation : BaseOperation
    {
        private const string _originalRoutingKey = "ORG_RoutingKey";
        internal string SourceQueueName;
        internal string DestinationExchangeName;

        internal override void Execute(IModel model, string prefix)
        {
            BasicGetResult message;
            while ((message = model.BasicGet(SourceQueueName, false)) != null)
            {
                // get routingKey from props header original routingKey or use routingKey from message itself
                var props = message.BasicProperties;
                var routingKey = props.Headers.ContainsKey(_originalRoutingKey)
                    ? Encoding.UTF8.GetString(props.Headers[_originalRoutingKey] as byte[])
                    : message.RoutingKey;

                model.BasicPublish(DestinationExchangeName, routingKey, props, message.Body);
                model.BasicAck(message.DeliveryTag, false);
            }
        }

        internal MoveDataToExchangeOperation SetSourceQueueName(string value)
        {
            SourceQueueName = value;
            return this;
        }

        internal MoveDataToExchangeOperation SetDestinationExchangeName(string value)
        {
            DestinationExchangeName = value;
            return this;
        }
    }
}
