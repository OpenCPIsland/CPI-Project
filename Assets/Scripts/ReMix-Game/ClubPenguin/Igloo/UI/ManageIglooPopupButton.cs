using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	public class ManageIglooPopupButton : MonoBehaviour
	{
		public void OnButtonClick()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IglooUIEvents.OpenManageIglooPopup));
		}
	}
}
