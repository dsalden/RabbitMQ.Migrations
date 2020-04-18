using System;

namespace RabbitMQ.Migrations.Attributes
{
    /// <summary>
    /// Indicates that a class is a <see cref="RabbitMqMigration" /> and provides its identifier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RabbitMqMigrationAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of this attribute.
        /// </summary>
        /// <param name="id">The migration identifier.</param>
        /// <param name="prefix">The prefix of this migration.</param>
        public RabbitMqMigrationAttribute(string id, string prefix = null)
        {
            Guard.ArgumentNotNullOrEmpty(nameof(id), id);

            Id = id;
            Prefix = prefix;
        }

        /// <summary>
        /// The migration identifier.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The prefix of this migration.
        /// </summary>
        public string Prefix { get; set; }
    }
}
