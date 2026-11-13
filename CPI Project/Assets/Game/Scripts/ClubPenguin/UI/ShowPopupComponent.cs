using ClubPenguin.Analytics;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ShowPopupComponent : MonoBehaviour
	{
		public PrefabContentKey PopupPrefabContentKey;

		public bool FullScreen = false;

		public string SwrveLogTier1 = "game.interactive_object";

		public string SwrveLogTier2;

		private bool isLoadingPrefab = false;

		private GameObject popupObject;

		public void ActivatePopup(GameObject sender)
		{
			if (sender.CompareTag("Player") && popupObject == null && !isLoadingPrefab)
			{
				isLoadingPrefab = true;
				if (!string.IsNullOrEmpty(SwrveLogTier2))
				{
					Service.Get<ICPSwrveService>().Action(SwrveLogTier1, SwrveLogTier2);
				}
				Content.LoadAsync(onPopupLoaded, PopupPrefabContentKey);
			}
		}

		private void onPopupLoaded(string path, GameObject popup)
		{
			isLoadingPrefab = false;
			popupObject = Object.Instantiate(popup);
			if (FullScreen)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowFullScreenPopup(popupObject));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popupObject));
			}
		}
	}
}
