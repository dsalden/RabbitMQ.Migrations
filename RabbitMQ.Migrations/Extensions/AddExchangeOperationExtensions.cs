using RabbitMQ.Migrations.Operations;

namespace RabbitMQ.Migrations.Extensions
{
    public static class AddExchangeOperationExtensions
    {
        public static AddExchangeOperation SetAlternativeExchange(this AddExchangeOperation operation, string value)
        {
            Guard.ArgumentNotNull(nameof(operation), operation);

            return operation.AddArgument("alternate-exchange", value);
        }
    }
}
