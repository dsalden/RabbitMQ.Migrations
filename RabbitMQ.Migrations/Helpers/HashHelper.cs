using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace RabbitMQ.Migrations.Helpers
{
    internal static class HashHelper
    {
        public static string ComputeSha256Hash(object data)
        {
            var rawData = JsonConvertHelper.SerializeObjectToByteArray(data);
            return ComputeSha256Hash(rawData);
        }

        public static string ComputeSha256Hash(byte[] rawData)
        {
            // Create a SHA256
            var builder = new StringBuilder();
            using var sha256Hash = SHA256.Create();
            // ComputeHash - returns byte array
            var bytes = sha256Hash.ComputeHash(rawData);

            // Convert byte array to a string
            foreach (var @byte in bytes)
            {
                builder.Append(@byte.ToString("x2", CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }
    }
}
