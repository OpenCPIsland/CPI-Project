using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Disney.Kelowna.Common
{
    public static class MD5HashUtil
    {
        private static MD5 md5;

        public static string GetHash(object data)
        {
            return calculateHash(data);
        }

        private static string calculateHash(object data)
        {
            byte[] buffer;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                JsonSerializer.Serialize(memoryStream, data, data.GetType());
                buffer = memoryStream.ToArray();
            }
            if (md5 == null)
            {
                md5 = MD5.Create();
            }
            byte[] bytes = md5.ComputeHash(buffer);
            return bytesToString(bytes);
        }

        private static string bytesToString(byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                stringBuilder.Append(bytes[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }
    }
}