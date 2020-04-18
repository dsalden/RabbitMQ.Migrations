using RabbitMQ.Client;
using System;
using Newtonsoft.Json;

namespace RabbitMQ.Migrations.Operations
{
    public class DeleteExchangeOperation : BaseOperation
    {
        [JsonProperty]
        internal string Name;
        [JsonProperty]
        internal bool IfUnused;

        internal override void Execute(IModel model, string prefix)
        {
            model.ExchangeDelete(GetName(prefix, Name), IfUnused);
        }

        internal DeleteExchangeOperation SetName(string value)
        {
            Name = value;
            return this;
        }

        /// <summary>
        /// Only delete the exchange if it is not used (does not have any consumers)
        /// </summary>
        public DeleteExchangeOperation SetIfUnused(bool value = false)
        {
            IfUnused = value;
            return this;
        }
    }
}
