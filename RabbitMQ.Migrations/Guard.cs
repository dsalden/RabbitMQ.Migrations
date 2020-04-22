using System;
using System.Collections.Generic;

namespace RabbitMQ.Migrations
{
    /// <summary>
    /// Allows a method to guard itself against incorrect parameters, that still pass through the typecheck. For example when a method gets <code>null</code> when you expect a <code>String</code>
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Guards against null values when a reference type is expected
        /// </summary>
        /// <param name="argumentName">Name of the argument</param>
        /// <param name="value">The received value</param>
        /// <exception cref="ArgumentNullException">When the argument contained a null value</exception>
        public static void ArgumentNotNull(string argumentName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Guards against null values when a String is expected. ALso guards against empty strings.
        /// </summary>
        /// <param name="argumentName">Name of the argument</param>
        /// <param name="value">The received String</param>
        /// <exception cref="ArgumentNullException">When the argument contained a null value</exception>
        /// <exception cref="ArgumentException">When the argument contained an empty String</exception>
        public static void ArgumentNotNullOrEmpty(string argumentName, string value)
        {
            ArgumentNotNull(argumentName, value);

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be an empty string.", argumentName);
            }
        }

        /// <summary>
        /// Guards against null values when a List is expected. Also guards against empty lists.
        /// </summary>
        /// <remarks>Use <see cref="IReadOnlyCollection{T}"/> instead of <see cref="IEnumerable{T}"/> to discourage double enumeration</remarks>
        /// <typeparam name="T">The type of the generic collection</typeparam>
        /// <param name="argumentName">Name of the argument</param>
        /// <param name="items">The received List.</param>
        /// <exception cref="ArgumentNullException">When the argument contained a null value</exception>
        /// <exception cref="ArgumentException">When the argument contained an empty List</exception>
        public static void ArgumentNotNullOrEmpty<T>(string argumentName, IReadOnlyCollection<T> items)
        {
            ArgumentNotNull(argumentName, items);

            if (items.Count == 0)
            {
                throw new ArgumentException("Collection must contain at least one item.", argumentName);
            }
        }

        /// <summary>
        /// Guards against unmet conditions for an specific argument
        /// </summary>
        /// <param name="valid">Condition</param>
        /// <param name="argumentName">Name of the argument</param>
        /// <param name="exceptionMessage">Message of the exception</param>
        /// <exception cref="ArgumentException">When the condition for the argument is not met.</exception>
        public static void ArgumentValid(bool valid, string argumentName, string exceptionMessage)
        {
            if (!valid)
            {
                throw new ArgumentException(exceptionMessage, argumentName);
            }
        }

        /// <summary>
        /// Guards against a not disposed object. An external boolean flag alongside the IDisposable object is needed, as IDisposable does not expose a way to determine the disposed state.
        /// </summary>
        /// <param name="disposed">The flag that specifies wether or not the object is disposed</param>
        /// <param name="objectName">The name of the object</param>
        /// <exception cref="ObjectDisposedException">When the flag states that the object is not disposed</exception>
        public static void NotDisposed(bool disposed, string objectName)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(objectName);
            }
        }

        /// <summary>
        /// Guards against unmet conditions
        /// </summary>
        /// <param name="valid">Condition</param>
        /// <param name="exceptionMessage">Message of the exception</param>
        /// <exception cref="InvalidOperationException">When the condition for the argument is not met.</exception>
        public static void OperationValid(bool valid, string exceptionMessage)
        {
            if (!valid)
            {
                throw new InvalidOperationException(exceptionMessage);
            }
        }
    }
}
