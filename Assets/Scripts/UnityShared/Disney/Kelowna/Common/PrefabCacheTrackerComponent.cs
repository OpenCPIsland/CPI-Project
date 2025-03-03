using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class PrefabCacheTrackerComponent : MonoBehaviour
	{
		public PrefabContentKey ContentKey
		{
			get;
			private set;
		}

		public event Action<PrefabCacheTrackerComponent> ObjectDestroyed;

		public void SetContentKey(PrefabContentKey contentKey)
		{
			ContentKey = contentKey;
		}

		private void OnDestroy()
		{
			if (this.ObjectDestroyed != null)
			{
				this.ObjectDestroyed(this);
			}
			this.ObjectDestroyed = null;
		}
	}
}
