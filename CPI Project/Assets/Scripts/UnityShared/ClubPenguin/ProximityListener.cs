using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class ProximityListener : MonoBehaviour
	{
		public string ProximityGroup;

		private bool setup = false;

		public virtual void OnProximityEnter(ProximityBroadcaster other)
		{
		}

		public virtual void OnProximityStay(ProximityBroadcaster other)
		{
		}

		public virtual void OnProximityExit(ProximityBroadcaster other)
		{
		}

		public virtual void Awake()
		{
		}

		public virtual void Start()
		{
			Service.Get<ProximityService>().AddListener(this);
			setup = true;
		}

		public virtual void OnDestroy()
		{
			if (setup)
			{
				Service.Get<ProximityService>().RemoveListener(this);
			}
		}
	}
}
