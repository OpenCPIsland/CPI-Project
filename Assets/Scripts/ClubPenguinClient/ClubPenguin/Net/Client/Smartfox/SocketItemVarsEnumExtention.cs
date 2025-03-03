using Disney.LaunchPadFramework;
using Sfs2X.Entities.Variables;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ClubPenguin.Net.Client.Smartfox
{
	public static class SocketItemVarsEnumExtention
	{
		private static Dictionary<SocketItemVars, string> keyMap;

		private static Dictionary<SocketItemVars, VariableType> typeMap;

		static SocketItemVarsEnumExtention()
		{
			keyMap = new Dictionary<SocketItemVars, string>();
			typeMap = new Dictionary<SocketItemVars, VariableType>();
			foreach (SocketItemVars value in Enum.GetValues(typeof(SocketItemVars)))
			{
				SocketItemVarsAttribute attr = GetAttr(value);
				keyMap.Add(value, attr.Key);
				typeMap.Add(value, attr.Type);
			}
		}

		public static string GetKey(this SocketItemVars v)
		{
			if (!keyMap.ContainsKey(v))
			{
				Log.LogError(v, string.Concat("Socket item var `", v, "` has no key"));
				throw new Exception(string.Concat("Socket item var `", v, "` has no key"));
			}
			return keyMap[v];
		}

		public static VariableType GetVariableType(this SocketItemVars v)
		{
			if (!typeMap.ContainsKey(v))
			{
				Log.LogError(v, string.Concat("Socket item var `", v, "` has no type"));
				throw new Exception(string.Concat("Socket item var `", v, "` has no type"));
			}
			return typeMap[v];
		}

		private static SocketItemVarsAttribute GetAttr(SocketItemVars v)
		{
			return (SocketItemVarsAttribute)Attribute.GetCustomAttribute(ForValue(v), typeof(SocketItemVarsAttribute));
		}

		private static MemberInfo ForValue(SocketItemVars v)
		{
			return typeof(SocketItemVars).GetField(Enum.GetName(typeof(SocketItemVars), v));
		}
	}
}
