﻿using RabbitMQ.Client;
using System;

namespace RabbitMQ.Migrations.Operations
{
    public class DeleteQueueOperation : BaseOperation
    {
        internal string Name;
        internal bool IfUnused;
        internal bool IfEmpty;

        internal override void Execute(IModel model, string prefix)
        {
            model.QueueDelete(GetName(prefix, Name), IfUnused, IfEmpty);
        }

        internal override int CalculateHash() => HashCode.Combine(Name, IfUnused, IfEmpty);

        internal DeleteQueueOperation SetName(string value)
        {
            Name = value;
            return this;
        }

        /// <summary>
        /// Only delete the queue if it is not used (does not have any consumers)
        /// </summary>
        public DeleteQueueOperation SetIfUnused(bool value = false)
        {
            IfUnused = value;
            return this;
        }

        /// <summary>
        /// Only delete the queue if it is empty
        /// </summary>
        public DeleteQueueOperation SetIfEmpty(bool value = false)
        {
            IfEmpty = value;
            return this;
        }
    }
}
