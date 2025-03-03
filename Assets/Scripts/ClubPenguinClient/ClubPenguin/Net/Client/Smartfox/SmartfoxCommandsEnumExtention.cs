using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ClubPenguin.Net.Client.Smartfox
{
	internal static class SmartfoxCommandsEnumExtention
	{
		private static Dictionary<SmartfoxCommand, string> commandMap;

		static SmartfoxCommandsEnumExtention()
		{
			commandMap = new Dictionary<SmartfoxCommand, string>();
			foreach (SmartfoxCommand value in Enum.GetValues(typeof(SmartfoxCommand)))
			{
				SmartfoxCommandsAttribute attr = GetAttr(value);
				commandMap.Add(value, attr.Command);
			}
		}

		public static string GetCommand(this SmartfoxCommand v)
		{
			if (!commandMap.ContainsKey(v))
			{
				Log.LogError(v, string.Concat("Smartfox command `", v, "` has no command"));
				throw new Exception(string.Concat("Smartfox command `", v, "` has no command"));
			}
			return commandMap[v];
		}

		private static SmartfoxCommandsAttribute GetAttr(SmartfoxCommand v)
		{
			return (SmartfoxCommandsAttribute)Attribute.GetCustomAttribute(ForValue(v), typeof(SmartfoxCommandsAttribute));
		}

		private static MemberInfo ForValue(SmartfoxCommand v)
		{
			return typeof(SmartfoxCommand).GetField(Enum.GetName(typeof(SmartfoxCommand), v));
		}
	}
}
