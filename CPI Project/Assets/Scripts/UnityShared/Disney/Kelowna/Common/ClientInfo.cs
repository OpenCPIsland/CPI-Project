using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class ClientInfo : ScriptableObject
	{
		public const string CLIENT_INFO_RUNTIME_PATH = "Configuration/client_info";

		public const string CLIENT_INFO_EDITOR_PATH = "Assets/Generated/Resources/Configuration/client_info.asset";

		public string ClientVersion;

		public string BuildVersion;

		public string Changelist;

		public string[] GameAssemblyNames;

		public string Platform;

		public string BuildTargetGroup;

		public string BuildPhase;

		private static ClientInfo instance;

		public string BuildTargetGroupWithPhase
		{
			get
			{
				return string.Format("{0}-{1}", BuildTargetGroup, BuildPhase);
			}
		}

		public static ClientInfo Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load<ClientInfo>("Configuration/client_info");
				}
				return instance;
			}
		}

		public static string GetClientVersionStr()
		{
			return Instance.ClientVersion;
		}

		public static Version ParseClientVersion(string clientVersionString)
		{
			clientVersionString = clientVersionString.Replace("v", string.Empty);
			return new Version(clientVersionString);
		}
	}
}
