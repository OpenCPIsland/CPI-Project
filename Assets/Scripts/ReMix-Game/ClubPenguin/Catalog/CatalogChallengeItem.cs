using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogChallengeItem : MonoBehaviour
	{
		public Image ButtonImage;

		public Image BackgroundImage;

		public Button CreatorButton;

		public Text CreatorText;

		public Text ThemeTitleText;

		public Text TopSellerTitleText;

		private CatalogThemeDefinition catalogChallengeTheme;

		private CurrentThemeData currentThemeData;

		private int themeIndex;

		public void SetChallengeTheme(CurrentThemeData theme, int index)
		{
			themeIndex = index;
			currentThemeData = theme;
			catalogChallengeTheme = Service.Get<CatalogServiceProxy>().GetThemeByScheduelId(theme.scheduledThemeChallengeId);
			Color[] colorsByIndex = Service.Get<CatalogServiceProxy>().themeColors.GetColorsByIndex(index);
			if (ButtonImage != null)
			{
				ButtonImage.color = colorsByIndex[0];
			}
			if (BackgroundImage != null)
			{
				BackgroundImage.color = colorsByIndex[1];
			}
			if (CreatorButton != null && CreatorText != null)
			{
				CreatorButton.gameObject.SetActive(false);
				if (theme.mostPopularItem.HasValue)
				{
					CreatorButton.gameObject.SetActive(true);
					CreatorText.text = theme.mostPopularItem.Value.creatorName;
				}
			}
			if (ThemeTitleText != null)
			{
				ThemeTitleText.text = Service.Get<Localizer>().GetTokenTranslation(catalogChallengeTheme.Title);
			}
			if (TopSellerTitleText != null)
			{
				TopSellerTitleText.enabled = false;
				if (theme.mostPopularItem.HasValue)
				{
					TopSellerTitleText.enabled = true;
				}
			}
		}

		public void OnCLick()
		{
			Service.Get<ICPSwrveService>().Action("clothing_catalog_theme", catalogChallengeTheme.Title);
			Service.Get<CatalogServiceProxy>().CurrentThemeIndex = themeIndex;
			CatalogContext.EventBus.DispatchEvent(new CatalogUIEvents.ShowItemsForThemeEvent(currentThemeData, -1L));
		}

		public void OnCreatorButtonClicked()
		{
			if (currentThemeData.mostPopularItem.HasValue)
			{
				CPDataEntityCollection dataEntityCollection = Service.Get<CPDataEntityCollection>();
				DataEntityHandle handle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, currentThemeData.mostPopularItem.Value.creatorName);
				OpenPlayerCardCommand openPlayerCardCommand = new OpenPlayerCardCommand(handle);
				openPlayerCardCommand.Execute();
			}
		}
	}
}
