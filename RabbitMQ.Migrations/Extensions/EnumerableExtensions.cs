using System;
using System.Collections.Generic;

namespace RabbitMQ.Migrations.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            Guard.ArgumentNotNull(nameof(list), list);

            foreach (var t in list)
            {
                action(t);
            }
        }
    }
}
