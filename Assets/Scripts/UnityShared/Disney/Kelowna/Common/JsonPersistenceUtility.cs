using Disney.LaunchPadFramework;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class JsonPersistenceUtility
	{
		public static void WriteJsonData(string filePath, object obj)
		{
			string contents = JsonUtility.ToJson(obj);
			FileInfo fileInfo = new FileInfo(filePath);
			DirectoryInfo directory = fileInfo.Directory;
			if (!directory.Exists)
			{
				Directory.CreateDirectory(directory.FullName);
			}
			File.WriteAllText(filePath, contents);
		}

		public static void ClearJsonData(string filePath)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		public static T ReadJsonData<T>(string filePath)
		{
			string json = readJsonData(filePath);
			return JsonUtility.FromJson<T>(json);
		}

		public static void ReadJsonDataOverwrite(string filePath, object objectToOverwrite)
		{
			string json = readJsonData(filePath);
			JsonUtility.FromJsonOverwrite(json, objectToOverwrite);
		}

		public static bool TryReadJsonData<T>(string filePath, out T jsonObject)
		{
			jsonObject = default(T);
			string text = readJsonData(filePath, false);
			if (!string.IsNullOrEmpty(text))
			{
				jsonObject = JsonUtility.FromJson<T>(text);
				return jsonObject != null;
			}
			return false;
		}

		private static string readJsonData(string filePath, bool logError = true)
		{
			if (File.Exists(filePath))
			{
				return File.ReadAllText(filePath);
			}
			if (logError)
			{
				if (string.IsNullOrEmpty(filePath))
				{
					Log.LogError(typeof(JsonPersistenceUtility), "File path was null or empty");
				}
				else
				{
					Log.LogError(typeof(JsonPersistenceUtility), "File not found at path " + filePath);
				}
			}
			return null;
		}
	}
}
