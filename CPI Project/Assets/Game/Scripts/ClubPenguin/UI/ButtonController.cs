using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ButtonController : MonoBehaviour
	{
		public Text ButtonText;

		public Image ButtonImage;

		private DButton buttonData;

		public System.Action EButtonLoadCompleteAction;

		public void ShowButton(DButton buttonData)
		{
			this.buttonData = buttonData;
			if (!string.IsNullOrEmpty(buttonData.Text))
			{
				ButtonText.text = buttonData.Text;
			}
			if (!string.IsNullOrEmpty(buttonData.IconKey))
			{
				CoroutineRunner.Start(loadButtonIcon(buttonData.IconKey), this, "loadButtonIcon");
			}
			else
			{
				if (ButtonImage != null)
				{
					ButtonImage.enabled = false;
				}
				if (EButtonLoadCompleteAction != null)
				{
					EButtonLoadCompleteAction();
				}
			}
			addCLickListener();
		}

		private void addCLickListener()
		{
			Button component = GetComponent<Button>();
			if (component == null)
			{
			}
			component.onClick.AddListener(onButtonClick);
		}

		private void onButtonClick()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new ButtonEvents.ClickEvent(buttonData));
		}

		private IEnumerator loadButtonIcon(string contentKey)
		{
			AssetRequest<Sprite> assetRequest = Content.LoadAsync<Sprite>(contentKey);
			yield return assetRequest;
			ButtonImage.sprite = assetRequest.Asset;
			ButtonImage.enabled = true;
			EButtonLoadCompleteAction();
		}

		public void OnDestroy()
		{
			GetComponent<Button>().onClick.RemoveListener(onButtonClick);
		}
	}
}
