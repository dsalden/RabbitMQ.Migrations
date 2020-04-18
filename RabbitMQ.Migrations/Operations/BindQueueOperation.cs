using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RabbitMQ.Migrations.Operations
{
    public class BindQueueOperation : BaseOperation
    {
        [JsonProperty]
        internal string QueueName;
        [JsonProperty]
        internal string ExchangeName;
        [JsonProperty]
        internal string RoutingKey;
        [JsonProperty]
        internal readonly IDictionary<string, object> Arguments = new Dictionary<string, object>();

        internal override void Execute(IModel model, string prefix)
        {
            model.QueueBind(GetName(prefix, QueueName), GetName(prefix, ExchangeName), RoutingKey, Arguments);
        }

        internal BindQueueOperation SetQueueName(string value)
        {
            QueueName = value;
            return this;
        }

        internal BindQueueOperation SetExchangeName(string value)
        {
            ExchangeName = value;
            return this;
        }

        internal BindQueueOperation SetRoutingKey(string value)
        {
            RoutingKey = value;
            return this;
        }

        public BindQueueOperation AddArgument(string key, object value)
        {
            Arguments[key] = value;
            return this;
        }
    }
}
