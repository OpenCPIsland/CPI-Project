using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Canvas))]
	public class TopPopupManager : BasePopupManager
	{
		private void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PopupEvents.ShowTopPopup>(onShowPopup);
		}

		private bool onShowPopup(PopupEvents.ShowTopPopup evt)
		{
			showPopup(evt.Popup, evt.DestroyPopupOnBackPressed, evt.ScaleToFit);
			return false;
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}
	}
}
