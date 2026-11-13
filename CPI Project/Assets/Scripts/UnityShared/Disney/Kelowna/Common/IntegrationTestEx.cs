using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class IntegrationTestEx
	{
		public static void FailIf(bool condition)
		{
			FailIf(condition, "");
		}

		public static void FailIf(bool condition, string message)
		{
			if (condition)
			{
				IntegrationTest.Fail(message);
			}
		}

		public static void FailIf(GameObject go, bool condition)
		{
			FailIf(go, condition, "");
		}

		public static void FailIf(GameObject go, bool condition, string message)
		{
			if (condition)
			{
				IntegrationTest.Fail(go, message);
			}
		}
	}
}
