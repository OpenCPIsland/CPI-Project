using System.Diagnostics;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.Utility.Assert
{
	public class Assert
	{
		[Conditional("UNITY_EDITOR")]
		public static void That(bool comparison, string message)
		{
			if (!comparison)
			{
				Logger.LogWarning(typeof(Assert), message);
				UnityEngine.Debug.LogWarning(message);
				UnityEngine.Debug.Break();
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void Throw(string message)
		{
			Logger.LogWarning(typeof(Assert), message);
			UnityEngine.Debug.LogWarning(message);
		}
	}
}
