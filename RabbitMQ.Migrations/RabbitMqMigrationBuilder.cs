using RabbitMQ.Client;
using RabbitMQ.Migrations.Operations;
using System.Collections.Generic;

namespace RabbitMQ.Migrations
{
    public class RabbitMqMigrationBuilder
    {
        public IList<BaseOperation> Operations { get; } = new List<BaseOperation>();

        public AddExchangeOperation AddDirectExchange(string name) => AddExchange(name, ExchangeType.Direct);
        public AddExchangeOperation AddFanoutExchange(string name) => AddExchange(name, ExchangeType.Fanout);
        public AddExchangeOperation AddHeadersExchange(string name) => AddExchange(name, ExchangeType.Headers);
        public AddExchangeOperation AddTopicExchange(string name) => AddExchange(name, ExchangeType.Topic);

        public AddExchangeOperation AddExchange(string name, string type)
        {
            var addExchangeOperation = new AddExchangeOperation()
                .SetName(name)
                .SetType(type);

            Operations.Add(addExchangeOperation);

            return addExchangeOperation;
        }

        public DeleteExchangeOperation DeleteExchange(string name)
        {
            var deleteExchangeOperation = new DeleteExchangeOperation()
                .SetName(name);

            Operations.Add(deleteExchangeOperation);

            return deleteExchangeOperation;
        }

        public BindExchangeOperation BindExchange(string sourceExchangeName, string destinationExchangeName, string routingKey)
        {
            var bindExchangeOperation = new BindExchangeOperation()
                .SetSourceExchangeName(sourceExchangeName)
                .SetDestinationExchangeName(destinationExchangeName)
                .SetRoutingKey(routingKey);

            Operations.Add(bindExchangeOperation);

            return bindExchangeOperation;
        }

        public UnbindExchangeOperation UnbindExchange(string sourceExchangeName, string destinationExchangeName, string routingKey)
        {
            var unbindExchangeOperation = new UnbindExchangeOperation()
                .SetSourceExchangeName(sourceExchangeName)
                .SetDestinationExchangeName(destinationExchangeName)
                .SetRoutingKey(routingKey);

            Operations.Add(unbindExchangeOperation);

            return unbindExchangeOperation;
        }

        public AddQueueOperation AddQueue(string name)
        {
            var addQueueOperation = new AddQueueOperation()
                .SetName(name);

            Operations.Add(addQueueOperation);

            return addQueueOperation;
        }

        public DeleteQueueOperation DeleteQueue(string name)
        {
            var deleteQueueOperation = new DeleteQueueOperation()
                .SetName(name);

            Operations.Add(deleteQueueOperation);

            return deleteQueueOperation;
        }

        public BindQueueOperation BindQueue(string queueName, string exchangeName, string routingKey)
        {
            var bindQueueOperation = new BindQueueOperation()
                .SetQueueName(queueName)
                .SetExchangeName(exchangeName)
                .SetRoutingKey(routingKey);

            Operations.Add(bindQueueOperation);

            return bindQueueOperation;
        }

        public UnbindQueueOperation UnbindQueue(string queueName, string exchangeName, string routingKey)
        {
            var unbindQueueOperation = new UnbindQueueOperation()
                .SetQueueName(queueName)
                .SetExchangeName(exchangeName)
                .SetRoutingKey(routingKey);

            Operations.Add(unbindQueueOperation);

            return unbindQueueOperation;
        }

        public MoveDataToQueueOperation MoveDataToQueue(string sourceQueueName, string destinationQueueName)
        {
            var moveDataToQueueOperation = new MoveDataToQueueOperation()
                .SetSourceQueueName(sourceQueueName)
                .SetDestinationQueueName(destinationQueueName);

            Operations.Add(moveDataToQueueOperation);

            return moveDataToQueueOperation;
        }

        public MoveDataToExchangeOperation MoveDataToExchange(string sourceQueueName, string destinationExchangeName)
        {
            var moveDataToExchangeOperation = new MoveDataToExchangeOperation()
                .SetSourceQueueName(sourceQueueName)
                .SetDestinationExchangeName(destinationExchangeName);

            Operations.Add(moveDataToExchangeOperation);

            return moveDataToExchangeOperation;
        }
    }
}
