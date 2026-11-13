using System.Collections;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class LaunchDataHelper
	{
		public const string COMMAND_LINE_ARGS_KEY = "launchDataPath";

		public static string GetLaunchDataWritePath()
		{
			return Path.Combine(Application.persistentDataPath, "launch_data.json");
		}

		public static string GetLaunchDataReadPath()
		{
			string result = null;
			Hashtable commandLineArgs = CommandLineArgsUtils.GetCommandLineArgs();
			if (commandLineArgs.ContainsKey("launchDataPath"))
			{
				result = (string)commandLineArgs["launchDataPath"];
			}
			return result;
		}
	}
}
