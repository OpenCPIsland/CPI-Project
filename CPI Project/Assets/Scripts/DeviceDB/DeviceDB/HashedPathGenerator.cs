using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DeviceDB
{
	internal static class HashedPathGenerator
	{
		private static readonly HashAlgorithm hashAlgorithm = SHA1.Create();

		private static readonly Encoding stringEncoding = Encoding.UTF8;

		public static string GetPath(string dir, string entityName)
		{
			byte[] bytes = stringEncoding.GetBytes(entityName);
			byte[] inArray = hashAlgorithm.ComputeHash(bytes);
			string path = Convert.ToBase64String(inArray, Base64FormattingOptions.None).Replace('/', '_').Replace('=', '~')
				.Replace('+', '-');
			return Path.Combine(dir, path);
		}
	}
}
