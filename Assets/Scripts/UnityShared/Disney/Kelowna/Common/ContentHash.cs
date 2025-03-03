using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Disney.Kelowna.Common
{
	public class ContentHash
	{
		public static string CalculateHashForFile(bool includeFilenameInHash, string filename)
		{
			return CalculateHashForFiles(includeFilenameInHash, new string[1]
			{
				filename
			});
		}

		public static string CalculateHashForFiles(bool includeFilenameInHash, IEnumerable<string> filenames)
		{
			long num = 0L;
			string result = "";
			byte[] array = new byte[8192];
			foreach (string filename in filenames)
			{
				FileInfo fileInfo = new FileInfo(filename);
				num += fileInfo.Length;
			}
			using (HashAlgorithm hashAlgorithm = createHashAlgorithm())
			{
				addContentPrefix(hashAlgorithm, num);
				foreach (string filename2 in filenames)
				{
					string current = filename2.Replace('\\', '/');
					if (includeFilenameInHash)
					{
						Buffer.BlockCopy(current.ToCharArray(), 0, array, 0, current.Length);
						hashAlgorithm.TransformBlock(array, 0, current.Length, array, 0);
					}
					using (Stream stream = File.OpenRead(current))
					{
						int inputCount;
						while ((inputCount = stream.Read(array, 0, array.Length)) > 0)
						{
							hashAlgorithm.TransformBlock(array, 0, inputCount, array, 0);
						}
					}
				}
				hashAlgorithm.TransformFinalBlock(array, 0, 0);
				result = BytesToHexString(hashAlgorithm.Hash);
			}
			return result;
		}

		public static string CaluclateHashForUTF8String(string utf8String)
		{
			if (utf8String == null)
			{
				return null;
			}
			long totalBytes = utf8String.Length;
			string result = "";
			using (HashAlgorithm hashAlgorithm = createHashAlgorithm())
			{
				addContentPrefix(hashAlgorithm, totalBytes);
				byte[] bytes = Encoding.UTF8.GetBytes(utf8String);
				hashAlgorithm.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
				hashAlgorithm.TransformFinalBlock(bytes, 0, 0);
				result = BytesToHexString(hashAlgorithm.Hash);
			}
			return result;
		}

		public static string DeterministicOrderedJson(string genericJsonString)
		{
			if (genericJsonString == null)
			{
				return null;
			}
			JsonData jsonData = JsonMapper.ToObject(genericJsonString);
			jsonData = deterministicallySortJsonData(jsonData);
			JsonWriter jsonWriter = new JsonWriter();
			jsonWriter.PrettyPrint = false;
			JsonMapper.ToJson(jsonData, jsonWriter);
			return jsonWriter.ToString();
		}

		private static JsonData deterministicallySortJsonData(JsonData jsonData)
		{
			if (jsonData != null)
			{
				if (jsonData.IsArray)
				{
					for (int i = 0; i < jsonData.Count; i++)
					{
						jsonData[i] = deterministicallySortJsonData(jsonData[i]);
					}
				}
				else if (jsonData.IsObject)
				{
					List<string> list = new List<string>(jsonData.Keys);
					list.Sort(StringComparer.Ordinal);
					JsonData jsonData2 = new JsonData();
					jsonData2.SetJsonType(JsonType.Object);
					for (int i = 0; i < list.Count; i++)
					{
						string prop_name = list[i];
						JsonData jsonData4 = jsonData2[prop_name] = deterministicallySortJsonData(jsonData[prop_name]);
					}
					jsonData = jsonData2;
				}
			}
			return jsonData;
		}

		private static HashAlgorithm createHashAlgorithm()
		{
			return new SHA1CryptoServiceProvider();
		}

		private static void addContentPrefix(HashAlgorithm hashAlgorithm, long totalBytes)
		{
			string text = string.Format("clubpenguin {0}", totalBytes);
			byte[] array = new byte[text.Length + 1];
			Buffer.BlockCopy(text.ToCharArray(), 0, array, 0, text.Length);
			array[text.Length] = 0;
			hashAlgorithm.TransformBlock(array, 0, text.Length + 1, array, 0);
		}

		private static string BytesToHexString(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
			for (int i = 0; i < bytes.Length; i++)
			{
				stringBuilder.AppendFormat("{0:x2}", bytes[i]);
			}
			return stringBuilder.ToString();
		}
	}
}
