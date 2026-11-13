using System.Collections;

namespace Disney.LaunchPadFramework.Utility
{
	public static class CoroutineHelper
	{
		public static void RunCoroutineToCompletion(IEnumerator coroutineEnumerator)
		{
			do
			{
			}
			while (coroutineEnumerator == null || coroutineEnumerator.MoveNext());
		}
	}
}
