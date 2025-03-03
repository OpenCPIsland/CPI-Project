using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	[RequireComponent(typeof(Button))]
	public class IglooLotEditButton : MonoBehaviour
	{
		public void OnButtonClick()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IglooUIEvents.LotNextButtonPressed));
		}
	}
}
