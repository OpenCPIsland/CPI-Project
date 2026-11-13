using ClubPenguin.CellPhone;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class FriendsCellPhoneScreen : MonoBehaviour
	{
		public PrefabContentKey CellPhonePrefabKey;

		private bool isClosing;

		private void Start()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.FriendsLoadComplete));
		}

		public void OnCloseButtonClick()
		{
			if (!isClosing)
			{
				isClosing = true;
				Content.LoadAsync(onCellPhonePrefabLoaded, CellPhonePrefabKey);
			}
		}

		private void onCellPhonePrefabLoaded(string path, GameObject cellPhonePrefab)
		{
			GameObject popup = Object.Instantiate(cellPhonePrefab);
			PopupEvents.ShowCameraSpacePopup evt = new PopupEvents.ShowCameraSpacePopup(popup, false, true, "Accessibility.Popup.Title.CellPhone", "MainCamera", 1f, 0);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
			StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
			componentInParent.SendEvent(new ExternalEvent("Root", "noui"));
		}
	}
}
