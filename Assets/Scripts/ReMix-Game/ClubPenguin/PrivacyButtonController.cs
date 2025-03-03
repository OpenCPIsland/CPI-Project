using ClubPenguin.Igloo;
using ClubPenguin.Net.Domain.Igloo;
using Disney.Kelowna.Common;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class PrivacyButtonController : MonoBehaviour
	{
		private const int PUBLIC_BUTTON_INDEX = 0;

		private const int PRIVATE_BUTTON_INDEX = 1;

		private const int LOADING_BUTTON_INDEX = 2;

		private const int BUTTON_ENABLED_COLOR_INDEX = 0;

		private const int BUTTON_DISABLED_COLOR_INDEX = 1;

		public PrefabContentKey SelectionPrefab;

		public SpriteSelector SpriteSelector;

		private SavedIgloosMetaData savedIgloosMetaData;

		private bool isLoading;

		private GameObject popupInstance;

		private Transform popupParent;

		private Button button;

		private TintSelector buttonTintSelector;

		private bool popupWasClosed = false;

		public bool PopupWasClosed
		{
			set
			{
				popupWasClosed = value;
			}
		}

		private void Awake()
		{
			button = GetComponent<Button>();
			buttonTintSelector = button.GetComponentInChildren<TintSelector>();
			isLoading = true;
		}

		private void OnDestroy()
		{
			if (savedIgloosMetaData != null)
			{
				savedIgloosMetaData.PublishedStatusUpdated -= onPublishedStatusUpdated;
			}
			if (popupInstance != null)
			{
				Object.Destroy(popupInstance);
			}
		}

		public void Setup(Transform popupParent, SavedIgloosMetaData savedIgloosMetaData, bool enableButton)
		{
			this.popupParent = popupParent;
			if (this.savedIgloosMetaData != null)
			{
				this.savedIgloosMetaData.PublishedStatusUpdated -= onPublishedStatusUpdated;
			}
			this.savedIgloosMetaData = savedIgloosMetaData;
			savedIgloosMetaData.PublishedStatusUpdated += onPublishedStatusUpdated;
			onPublishedStatusUpdated(savedIgloosMetaData.IglooVisibility);
			isLoading = false;
			button.interactable = enableButton;
			if (buttonTintSelector != null)
			{
				int index = (!enableButton) ? 1 : 0;
				buttonTintSelector.SelectColor(index);
			}
		}

		public void OnButtonClicked()
		{
			if (!isLoading && savedIgloosMetaData != null && !popupWasClosed)
			{
				if (popupInstance != null)
				{
					Object.Destroy(popupInstance);
					return;
				}
				isLoading = true;
				SpriteSelector.SelectSprite(2);
				Content.LoadAsync(onPrefabLoaded, SelectionPrefab);
			}
		}

		private void onPrefabLoaded(string path, GameObject asset)
		{
			if (!popupWasClosed)
			{
				popupInstance = Object.Instantiate(asset, popupParent);
				onDataLoaded();
			}
		}

		private void onDataLoaded()
		{
			isLoading = false;
			onPublishedStatusUpdated(savedIgloosMetaData.IglooVisibility);
		}

		private void onPublishedStatusUpdated(IglooVisibility status)
		{
			switch (status)
			{
			case IglooVisibility.FRIENDS_ONLY:
				break;
			case IglooVisibility.PRIVATE:
				SpriteSelector.SelectSprite(1);
				break;
			case IglooVisibility.PUBLIC:
				SpriteSelector.SelectSprite(0);
				break;
			}
		}
	}
}
