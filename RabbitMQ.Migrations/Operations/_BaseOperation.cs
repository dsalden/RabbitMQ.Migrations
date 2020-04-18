using RabbitMQ.Client;

namespace RabbitMQ.Migrations.Operations
{
    public abstract class BaseOperation
    {
        internal virtual void Execute(IConnection connection, string prefix)
        {
            using var model = connection.CreateModel();
            Execute(model, prefix);
        }

        internal abstract void Execute(IModel model, string prefix);

        protected static string GetName(string prefix, string name)
        {
            return string.IsNullOrEmpty(prefix)
                ? name
                : $"{prefix}.{name}";
        }
    }
}
