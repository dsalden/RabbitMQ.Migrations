using RabbitMQ.Migrations.Operations;

namespace RabbitMQ.Migrations.Extensions
{
    public static class AddQueueOperationExtensions
    {
        /// <summary>
        /// How long a message published to a queue can live before it is discarded (milliseconds).
        /// (Sets the "x-message-ttl" argument.)
        /// </summary>
        public static AddQueueOperation SetMessageTimeToLive(this AddQueueOperation operation, int value)
        {
            return operation.AddArgument("x-message-ttl", value);
        }

        /// <summary>
        /// How long a queue can be unused for before it is automatically deleted (milliseconds).
        /// (Sets the "x-expires" argument.)
        /// </summary>
        public static AddQueueOperation SetAutoExpire(this AddQueueOperation operation, int value)
        {
            return operation.AddArgument("x-expires", value);
        }

        /// <summary>
        /// How many (ready) messages a queue can contain before it starts to drop them from its head.
        /// (Sets the "x-max-length" argument.)
        /// </summary>
        public static AddQueueOperation SetMaxLength(this AddQueueOperation operation, int value)
        {
            return operation.AddArgument("x-max-length", value);
        }

        /// <summary>
        /// Total body size for ready messages a queue can contain before it starts to drop them from its head.
        /// (Sets the "x-max-length-bytes" argument.)
        /// </summary>
        public static AddQueueOperation SetMaxLengthBytes(this AddQueueOperation operation, int value)
        {
            return operation.AddArgument("x-max-length-bytes", value);
        }

        /// <summary>
        /// Sets the queue overflow behaviour. This determines what happens to messages when the maximum length of a queue is reached.
        /// Valid values are 'drop-head' or 'reject-publish'.
        /// (Sets the "x-overflow" argument.)
        /// </summary>
        public static AddQueueOperation SetOverflowBehaviour(this AddQueueOperation operation, string value)
        {
            return operation.AddArgument("x-overflow", value);
        }

        /// <summary>
        /// Optional name of an exchange to which messages will be republished if they are rejected or expire.
        /// (Sets the "x-dead-letter-exchange" argument.)
        /// </summary>
        public static AddQueueOperation SetDeadLetterExchange(this AddQueueOperation operation, string value)
        {
            return operation.AddArgument("x-dead-letter-exchange", value);
        }

        /// <summary>
        /// Optional replacement routing key to use when a message is dead-lettered. If this is not set, the message's original routing key will be used.
        /// (Sets the "x-dead-letter-routing-key" argument.)
        /// </summary>
        public static AddQueueOperation SetDeadLetterRoutingKey(this AddQueueOperation operation, string value)
        {
            return operation.AddArgument("x-dead-letter-routing-key", value);
        }

        /// <summary>
        /// Maximum number of priority levels for the queue to support; if not set, the queue will not support message priorities.
        /// (Sets the "x-max-priority" argument.)
        /// </summary>
        public static AddQueueOperation SetMaxPriority(this AddQueueOperation operation, int value)
        {
            return operation.AddArgument("x-max-priority", value);
        }

        /// <summary>
        /// Set the queue into lazy mode, keeping as many messages as possible on disk to reduce RAM usage; if not set,
        /// the queue will keep an in-memory cache to deliver messages as fast as possible.
        /// (Sets the "x-queue-mode" argument.) 
        /// </summary>
        public static AddQueueOperation SetLazyMode(this AddQueueOperation operation)
        {
            return operation.AddArgument("x-queue-mode", "lazy");
        }

        /// <summary>
        /// Set the queue into master location mode, determining the rule by which the queue master is located when declared on a cluster of nodes.
        /// (Sets the "x-queue-master-locator" argument.)
        /// </summary>
        public static AddQueueOperation SetMasterLocation(this AddQueueOperation operation, string value)
        {
            return operation.AddArgument("x-queue-master-locator", value);
        }
    }
}
