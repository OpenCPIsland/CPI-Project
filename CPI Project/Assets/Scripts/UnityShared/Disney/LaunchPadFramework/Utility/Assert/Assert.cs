#define UNITY_ASSERTIONS
using System.Diagnostics;
using UnityEngine;

namespace Disney.LaunchPadFramework.Utility.Assert
{
	public class Assert
	{
		public static bool DidAssert;

		[Conditional("UNITY_EDITOR")]
		public static void That(bool comparison, string message)
		{
			if (!comparison)
			{
				DidAssert = true;
				UnityEngine.Debug.LogAssertion(message);
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void Throw(string message)
		{
			DidAssert = true;
			UnityEngine.Debug.LogAssertion(message);
		}
	}
}
