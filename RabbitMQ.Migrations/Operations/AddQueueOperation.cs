using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RabbitMQ.Migrations.Operations
{
    public class AddQueueOperation : BaseOperation
    {
        [JsonProperty]
        internal string Name;
        [JsonProperty]
        internal bool Durable;
        [JsonProperty]
        internal bool Exclusive;
        [JsonProperty]
        internal bool AutoDelete;
        [JsonProperty]
        internal readonly IDictionary<string, object> Arguments = new Dictionary<string, object>();
        [JsonProperty]
        internal readonly IList<BindQueueOperation> BindQueueOperations = new List<BindQueueOperation>();

        internal override void Execute(IModel model, string prefix)
        {
            model.QueueDeclare(GetName(prefix, Name), Durable, Exclusive, AutoDelete, Arguments);

            foreach (var bindQueueOperation in BindQueueOperations)
            {
                bindQueueOperation.Execute(model, prefix);
            }
        }

        internal AddQueueOperation SetName(string value)
        {
            Name = value;
            return this;
        }

        /// <summary>
        /// The queue will survive a broker restart
        /// </summary>
        public AddQueueOperation SetDurable(bool value = false)
        {
            Durable = value;
            return this;
        }

        /// <summary>
        /// The queue can only be used by one connection and the queue will be deleted when that connection closes
        /// </summary>
        public AddQueueOperation SetExclusive(bool value = true)
        {
            Exclusive = value;
            return this;
        }

        /// <summary>
        /// The queue is deleted when the last consumer unsubscribes
        /// </summary>
        public AddQueueOperation SetAutoDelete(bool value = true)
        {
            AutoDelete = value;
            return this;
        }

        public AddQueueOperation AddArgument(string key, object value)
        {
            Arguments[key] = value;
            return this;
        }

        public AddQueueOperation AddQueueBind(string exchangeName, string routingKey)
        {
            BindQueueOperations.Add(new BindQueueOperation()
                .SetQueueName(Name)
                .SetExchangeName(exchangeName)
                .SetRoutingKey(routingKey));
            return this;
        }
    }
}
