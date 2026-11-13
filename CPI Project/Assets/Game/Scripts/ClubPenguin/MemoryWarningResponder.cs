using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class MemoryWarningResponder
	{
		private const float TIME_BETWEEN_UNLOADS = 30f;

		private bool hadMemoryWarning = false;

		public void Init()
		{
			EnvironmentManager.LowMemoryEvent += onLowMemory;
			CoroutineRunner.StartPersistent(run(), this, "MemoryWarningResponder");
		}

		private void onLowMemory()
		{
			hadMemoryWarning = true;
		}

		private IEnumerator run()
		{
			while (true)
			{
				if (hadMemoryWarning)
				{
					hadMemoryWarning = false;
					Resources.UnloadUnusedAssets();
					yield return new WaitForSeconds(30f);
				}
				yield return null;
			}
		}
	}
}
