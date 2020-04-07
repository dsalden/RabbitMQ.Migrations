using System;
using System.Collections.Generic;

namespace RabbitMQ.Migrations.Extensions
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            Guard.ArgumentNotNull(nameof(list), list);
            Guard.ArgumentNotNull(nameof(items), items);

            if (list is List<T> asList)
            {
                asList.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }
    }
}
