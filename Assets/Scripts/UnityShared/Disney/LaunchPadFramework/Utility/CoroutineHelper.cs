using System.Collections;

namespace Disney.LaunchPadFramework.Utility
{
	public static class CoroutineHelper
	{
		public static void RunCoroutineToCompletion(IEnumerator coroutineEnumerator)
		{
			do
			{
				bool flag = true;
			}
			while (coroutineEnumerator == null || coroutineEnumerator.MoveNext());
		}
	}
}
