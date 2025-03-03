using System;

namespace ZenFulcrum.EmbeddedBrowser
{
	public interface IPromiseTimer2
	{
		IPromise2 WaitFor(float seconds);

		IPromise2 WaitUntil(Func<TimeData2, bool> predicate);

		IPromise2 WaitWhile(Func<TimeData2, bool> predicate);

		void Update(float deltaTime);
	}
}
