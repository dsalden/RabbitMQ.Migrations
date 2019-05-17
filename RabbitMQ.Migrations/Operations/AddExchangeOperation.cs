using System.Collections.Generic;
using RabbitMQ.Client;

namespace RabbitMQ.Migrations.Operations
{
    public class AddExchangeOperation : BaseOperation
    {
        internal string Name;
        internal string Type;
        internal bool Durable;
        internal bool AutoDelete;
        internal readonly IDictionary<string, object> Arguments = new Dictionary<string, object>();
        internal readonly IList<BindExchangeOperation> BindExchangeOperations = new List<BindExchangeOperation>();

        internal override void Execute(IModel model, string prefix)
        {
            model.ExchangeDeclare(GetName(prefix, Name), Type, Durable, AutoDelete, Arguments);

            foreach (var bindExchangeOperation in BindExchangeOperations)
            {
                bindExchangeOperation.Execute(model, prefix);
            }
        }

        internal AddExchangeOperation SetName(string value)
        {
            Name = value;
            return this;
        }

        internal AddExchangeOperation SetType(string value)
        {
            Type = value;
            return this;
        }

        public AddExchangeOperation SetDurable(bool value = false)
        {
            Durable = value;
            return this;
        }

        public AddExchangeOperation SetAutoDelete(bool value = false)
        {
            AutoDelete = value;
            return this;
        }

        public AddExchangeOperation AddArgument(string key, object value)
        {
            Arguments[key] = value;
            return this;
        }

        public AddExchangeOperation AddExchangeBind(string sourceExchangeName, string routingKey)
        {
            BindExchangeOperations.Add(new BindExchangeOperation()
                .SetDestinationExchangeName(Name)
                .SetSourceExchangeName(sourceExchangeName)
                .SetRoutingKey(routingKey));
            return this;
        }
    }
}
