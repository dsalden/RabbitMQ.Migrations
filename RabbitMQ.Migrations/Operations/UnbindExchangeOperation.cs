using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace RabbitMQ.Migrations.Operations
{
    public class UnbindExchangeOperation : BaseOperation
    {
        [JsonProperty]
        internal string SourceExchangeName;
        [JsonProperty]
        internal string DestinationExchangeName;
        [JsonProperty]
        internal string RoutingKey;
        [JsonProperty]
        internal readonly IDictionary<string, object> Arguments = new Dictionary<string, object>();

        internal override void Execute(IModel model, string prefix)
        {
            model.ExchangeUnbind(GetName(prefix, DestinationExchangeName), GetName(prefix, SourceExchangeName), RoutingKey, Arguments);
        }

        internal UnbindExchangeOperation SetDestinationExchangeName(string value)
        {
            DestinationExchangeName = value;
            return this;
        }

        internal UnbindExchangeOperation SetSourceExchangeName(string value)
        {
            SourceExchangeName = value;
            return this;
        }

        internal UnbindExchangeOperation SetRoutingKey(string value)
        {
            RoutingKey = value;
            return this;
        }

        public UnbindExchangeOperation AddArgument(string key, object value)
        {
            Arguments[key] = value;
            return this;
        }
    }
}
