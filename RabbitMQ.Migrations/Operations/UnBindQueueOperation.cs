using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace RabbitMQ.Migrations.Operations
{
    public class UnbindQueueOperation : BaseOperation
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
            model.QueueUnbind(GetName(prefix, QueueName), GetName(prefix, ExchangeName), RoutingKey, Arguments);
        }

        internal UnbindQueueOperation SetQueueName(string value)
        {
            QueueName = value;
            return this;
        }

        internal UnbindQueueOperation SetExchangeName(string value)
        {
            ExchangeName = value;
            return this;
        }

        internal UnbindQueueOperation SetRoutingKey(string value)
        {
            RoutingKey = value;
            return this;
        }

        public UnbindQueueOperation AddArgument(string key, object value)
        {
            Arguments[key] = value;
            return this;
        }
    }
}
