using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

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
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			byte[] buffer;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, data);
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
