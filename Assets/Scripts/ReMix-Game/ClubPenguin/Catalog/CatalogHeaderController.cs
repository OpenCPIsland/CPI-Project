using ClubPenguin.Avatar;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.Core;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogHeaderController : ACatalogController
	{
		public Text HeaderText;

		public Button BackButton;

		public Button CloseButton;

		public Button StatsButton;

		public string MyStatsToken;

		private EventChannel eventBus;

		private string defaultHeaderText;

		private AvatarRenderTextureComponent avatarRenderTextureComponent;

		private AvatarDetailsData avatarDetailsData;

		private void Awake()
		{
			defaultHeaderText = HeaderText.text;
			eventBus = new EventChannel(CatalogContext.EventBus);
			eventBus.AddListener<CatalogUIEvents.ShowItemsForThemeEvent>(onCatalogChallengeSelected);
			eventBus.AddListener<CatalogUIEvents.InvokeBackButtonClick>(onInvokeBackButtonClick);
			avatarRenderTextureComponent = StatsButton.GetComponent<AvatarRenderTextureComponent>();
			getLocalPlayerData();
		}

		private void getLocalPlayerData()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			avatarDetailsData = cPDataEntityCollection.GetComponent<AvatarDetailsData>(localPlayerHandle);
			if (avatarDetailsData == null)
			{
				Log.LogError(this, "Local player handle did not have avatar details data.");
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			eventBus.RemoveAllListeners();
		}

		private void OnEnable()
		{
			Service.Get<BackButtonController>().Add(onAndroidBackButtonClicked);
		}

		private void OnDisable()
		{
			Service.Get<BackButtonController>().Remove(onAndroidBackButtonClicked);
		}

		public void ShowStatsButton()
		{
			StatsButton.gameObject.SetActive(true);
			BackButton.gameObject.SetActive(false);
			if (avatarDetailsData != null)
			{
				avatarRenderTextureComponent.RenderAvatar(avatarDetailsData);
			}
			else
			{
				avatarRenderTextureComponent.RenderAvatar(new DCustomEquipment[0]);
			}
		}

		public void ShowBackButton()
		{
			StatsButton.gameObject.SetActive(false);
			BackButton.gameObject.SetActive(true);
		}

		private void onAndroidBackButtonClicked()
		{
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.InvokeBackButtonClick));
		}

		public bool onInvokeBackButtonClick(CatalogUIEvents.InvokeBackButtonClick evt)
		{
			if (BackButton.gameObject.activeSelf)
			{
				OnBackButton();
			}
			else
			{
				OnCloseButton();
			}
			return false;
		}

		public void OnBackButton()
		{
			HeaderText.text = defaultHeaderText;
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.BackButtonClicked));
			Model.State = CatalogState.Homepage;
		}

		public void OnCloseButton()
		{
			HeaderText.text = defaultHeaderText;
			Model.State = CatalogState.Homepage;
			ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.HideCatalog));
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.HideCatalog));
		}

		public void OnStatsButton()
		{
			Model.State = CatalogState.StatsView;
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.ShowStatsPage));
			HeaderText.text = Service.Get<Localizer>().GetTokenTranslation(MyStatsToken);
		}

		private bool onCatalogChallengeSelected(CatalogUIEvents.ShowItemsForThemeEvent evt)
		{
			long scheduledThemeChallengeId = evt.Theme.scheduledThemeChallengeId;
			CatalogThemeDefinition themeByScheduelId = Service.Get<CatalogServiceProxy>().GetThemeByScheduelId(scheduledThemeChallengeId);
			HeaderText.text = Service.Get<Localizer>().GetTokenTranslation(themeByScheduelId.Title);
			return false;
		}
	}
}
