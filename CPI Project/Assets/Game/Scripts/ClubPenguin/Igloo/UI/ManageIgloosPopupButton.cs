using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	[RequireComponent(typeof(Button))]
	internal class ManageIgloosPopupButton : MonoBehaviour
	{
		public void OnButton()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IglooUIEvents.OpenManageIglooPopup));
		}
	}
}
