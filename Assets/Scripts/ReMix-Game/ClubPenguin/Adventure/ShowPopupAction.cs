using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class ShowPopupAction : FsmStateAction
	{
		public string PrefabKey;

		public bool WaitForPopupComplete = false;

		public Vector2 PopupOffset;

		public Vector2 PopupScale = Vector2.one;

		public bool ShowCloseButton;

		public bool FullScreenClose;

		public bool ShowBackground;

		public bool IsCameraSpacePopup;

		private GameObject popupObject;

		private bool popupIsActive;

		private BasicPopup popup;

		public override void OnEnter()
		{
			Content.LoadAsync<GameObject>(PrefabKey, onPrefabLoaded);
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			popupObject = Object.Instantiate(prefab);
			if (IsCameraSpacePopup)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowCameraSpacePopup(popupObject));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popupObject));
			}
			popup = popupObject.GetComponent<BasicPopup>();
			if (popup != null)
			{
				DBasicPopup data = default(DBasicPopup);
				data.PopupOffset = PopupOffset;
				data.PopupScale = PopupScale;
				data.ShowBackground = ShowBackground;
				popup.SetData(data);
				popup.EnableCloseButtons(ShowCloseButton, FullScreenClose);
				popup.DoneClose += onPopupClosed;
			}
			if (!WaitForPopupComplete)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			if (popup != null)
			{
				popup.DoneClose -= onPopupClosed;
			}
		}

		private void onPopupClosed()
		{
			Finish();
		}
	}
}
