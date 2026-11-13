using UnityEngine;

namespace Disney.MobileNetwork
{
	public class VersionUtils : MonoBehaviour
	{
		public static string ParseClientVersion(string version)
		{
			string text = version;
			if (!string.IsNullOrEmpty(version))
			{
				if ((long)(text.Split('.').Length - 1) > 2L)
				{
					text = text.Substring(0, text.LastIndexOf("."));
				}
				else if (text.IndexOf("-") > 0)
				{
					text = text.Substring(0, text.IndexOf("-"));
				}
			}
			return text;
		}
	}
}
