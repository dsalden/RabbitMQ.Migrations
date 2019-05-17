using System.Collections.Generic;
using RabbitMQ.Client;

namespace RabbitMQ.Migrations.Operations
{
    public class BindExchangeOperation : BaseOperation
    {
        internal string SourceExchangeName;
        internal string DestinationExchangeName;
        internal string RoutingKey;
        internal readonly IDictionary<string, object> Arguments = new Dictionary<string, object>();

        internal override void Execute(IModel model, string prefix)
        {
            model.ExchangeBind(GetName(prefix, DestinationExchangeName), GetName(prefix, SourceExchangeName), RoutingKey, Arguments);
        }

        internal BindExchangeOperation SetDestinationExchangeName(string value)
        {
            DestinationExchangeName = value;
            return this;
        }

        internal BindExchangeOperation SetSourceExchangeName(string value)
        {
            SourceExchangeName = value;
            return this;
        }

        internal BindExchangeOperation SetRoutingKey(string value)
        {
            RoutingKey = value;
            return this;
        }

        public BindExchangeOperation AddArgument(string key, object value)
        {
            Arguments[key] = value;
            return this;
        }
    }
}
