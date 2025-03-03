using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class HudInitNotifier : MonoBehaviour
	{
		private IEnumerator Start()
		{
			yield return null;
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HudInitComplete));
		}
	}
}
