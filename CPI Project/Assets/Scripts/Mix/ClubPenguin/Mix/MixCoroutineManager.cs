using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin.Mix
{
	internal class MixCoroutineManager : ICoroutineManager
	{
		private Dictionary<IEnumerator, ICoroutine> activeCoroutines = new Dictionary<IEnumerator, ICoroutine>();

		public void Start(IEnumerator enumerator)
		{
			activeCoroutines.Add(enumerator, CoroutineRunner.StartPersistent(enumerator, this, "MixCoroutine"));
		}

		public void Stop(IEnumerator enumerator)
		{
			ICoroutine value;
			if (activeCoroutines.TryGetValue(enumerator, out value))
			{
				value.Cancel();
				return;
			}
			Service.Get<CoroutineRunner>().StopCoroutine(enumerator);
			Log.LogError(this, "Attempted to stop a mix coroutine that was not started via MixCoroutineManager. This may indicate that there is a bug in the Mix SDK.");
		}
	}
}
