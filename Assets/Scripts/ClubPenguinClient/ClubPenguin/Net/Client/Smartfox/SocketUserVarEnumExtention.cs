using Disney.LaunchPadFramework;
using Sfs2X.Entities.Variables;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ClubPenguin.Net.Client.Smartfox
{
	public static class SocketUserVarEnumExtention
	{
		private static Dictionary<SocketUserVars, string> keyMap;

		private static Dictionary<SocketUserVars, VariableType> typeMap;

		static SocketUserVarEnumExtention()
		{
			keyMap = new Dictionary<SocketUserVars, string>();
			typeMap = new Dictionary<SocketUserVars, VariableType>();
			foreach (SocketUserVars value in Enum.GetValues(typeof(SocketUserVars)))
			{
				SocketUserVarAttribute attr = GetAttr(value);
				keyMap.Add(value, attr.Key);
				typeMap.Add(value, attr.Type);
			}
		}

		public static string GetKey(this SocketUserVars v)
		{
			if (!keyMap.ContainsKey(v))
			{
				Log.LogError(v, string.Concat("Socket user var `", v, "` has no key"));
				throw new Exception(string.Concat("Socket user var `", v, "` has no key"));
			}
			return keyMap[v];
		}

		public static VariableType GetVariableType(this SocketUserVars v)
		{
			if (!typeMap.ContainsKey(v))
			{
				Log.LogError(v, string.Concat("Socket user var `", v, "` has no type"));
				throw new Exception(string.Concat("Socket user var `", v, "` has no type"));
			}
			return typeMap[v];
		}

		private static SocketUserVarAttribute GetAttr(SocketUserVars v)
		{
			return (SocketUserVarAttribute)Attribute.GetCustomAttribute(ForValue(v), typeof(SocketUserVarAttribute));
		}

		private static MemberInfo ForValue(SocketUserVars v)
		{
			return typeof(SocketUserVars).GetField(Enum.GetName(typeof(SocketUserVars), v));
		}
	}
}
