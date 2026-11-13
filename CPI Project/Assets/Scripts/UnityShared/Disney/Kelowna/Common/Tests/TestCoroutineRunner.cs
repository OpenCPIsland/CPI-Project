using System.Collections;

namespace Disney.Kelowna.Common.Tests
{
	public class TestCoroutineRunner : CoroutineRunner
	{
		public ICoroutine StartTestCoroutine(IEnumerator enumerator, object owner, string debugName)
		{
			return start(enumerator, owner, debugName, "T", persistentOwnerMap);
		}
	}
}
