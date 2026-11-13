using System.Collections;

namespace DisneyMobile.CoreUnitySystems.Utility
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
