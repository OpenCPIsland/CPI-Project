using System;
using System.IO;

namespace Disney.LaunchPadFramework.Utility
{
	public class FileHelper
	{
		public static int CountLinesInFile(string filePath)
		{
			int num = 0;
			using (StreamReader streamReader = new StreamReader(filePath))
			{
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					num++;
				}
			}
			return num;
		}

		public static void ClearFile(string filePath)
		{
			File.WriteAllText(filePath, string.Empty);
		}

		public static bool DeleteIfExists(string filePath)
		{
			if (File.Exists(filePath))
			{
				try
				{
					File.Delete(filePath);
				}
				catch (Exception)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}
}
