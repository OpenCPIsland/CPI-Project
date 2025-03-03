using System.Collections;

namespace DisneyMobile.CoreUnitySystems.Utility
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
