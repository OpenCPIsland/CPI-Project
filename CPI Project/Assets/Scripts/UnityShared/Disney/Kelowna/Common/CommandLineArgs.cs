using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class CommandLineArgs
	{
		public static string GetValueForKey(string key)
		{
			string result = null;
			string[] commandLineArgs = System.Environment.GetCommandLineArgs();
			try
			{
				string[] array = commandLineArgs;
				foreach (string text in array)
				{
					if (text.Contains(key + "="))
					{
						result = text.Split('=')[1];
					}
				}
			}
			catch (Exception)
			{
				Debug.LogError("Unable to parse arguments!");
			}
			return result;
		}
	}
}
