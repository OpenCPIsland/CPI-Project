using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	public class ClearCurrentLayoutButton : MonoBehaviour
	{
		public void Click()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IglooUIEvents.ClearCurrentLayout));
		}
	}
}
