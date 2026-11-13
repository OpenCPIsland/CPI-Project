using System.Collections;

namespace Disney.Mix.SDK
{
	public interface ICoroutineManager
	{
		void Start(IEnumerator enumerator);

		void Stop(IEnumerator enumerator);
	}
}
