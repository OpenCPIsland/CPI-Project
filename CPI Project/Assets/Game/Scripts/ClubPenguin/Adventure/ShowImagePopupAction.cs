using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ShowImagePopupAction : FsmStateAction
	{
		[RequiredField]
		public string PopupPrefabKey = "Prefabs/Popups/ImagePopup";

		[RequiredField]
		public string ImageAssetPath;

		public Vector2 ImageOffset;

		public Vector2 ImageScale = Vector2.one;

		public bool HasText;

		public string Text;

		public string i18nText;

		public DTextStyle TextStyle = new DTextStyle();

		public TextAnchor TextAlignment;

		public Vector2 TextOffset;

		public bool ShowCloseButton = true;

		public bool FullScreenClose = true;

		public bool ShowBackground = false;

		public bool WaitForPopupComplete = true;

		public float OpenDelay = 0f;

		private GameObject popup;

		private ImagePopup imagePopup;

		public override void OnEnter()
		{
			Content.LoadAsync<GameObject>(PopupPrefabKey, onPrefabLoaded);
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			DImagePopup dImagePopup = new DImagePopup();
			dImagePopup.ImageContentKey = new SpriteContentKey(ImageAssetPath);
			dImagePopup.ImageOffset = ImageOffset;
			dImagePopup.ImageScale = ImageScale;
			if (!HasText)
			{
				dImagePopup.Text = "";
			}
			else if (string.IsNullOrEmpty(i18nText))
			{
				dImagePopup.Text = Text;
			}
			else
			{
				dImagePopup.Text = i18nText;
			}
			dImagePopup.TextStyle = TextStyle;
			dImagePopup.TextAlignment = TextAlignment;
			dImagePopup.TextOffset = TextOffset;
			popup = Object.Instantiate(prefab);
			imagePopup = popup.GetComponent<ImagePopup>();
			if (imagePopup != null)
			{
				imagePopup.SetData(dImagePopup);
				imagePopup.EnableCloseButtons(ShowCloseButton, FullScreenClose);
				imagePopup.ShowBackground = ShowBackground;
				imagePopup.OpenDelay = OpenDelay;
				imagePopup.DoneClose += onPopupClosed;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popup));
			if (!WaitForPopupComplete)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			if (imagePopup != null)
			{
				imagePopup.DoneClose -= onPopupClosed;
			}
		}

		private void onPopupClosed()
		{
			Finish();
		}
	}
}
