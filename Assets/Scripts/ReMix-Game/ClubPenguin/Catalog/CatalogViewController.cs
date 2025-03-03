using ClubPenguin.ClothingDesigner.ItemCustomizer;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.Catalog
{
	public class CatalogViewController : MonoBehaviour
	{
		private CatalogHomePageController homePageController;

		private CatalogShopController shopController;

		private CatalogHeaderController headerController;

		private CatalogStatsPageController statsController;

		private CatalogSubnavController subnavController;

		private EventChannel customizerEventBus;

		private bool dependenciesSet;

		private void Awake()
		{
			customizerEventBus = new EventChannel(CustomizationContext.EventBus);
			customizerEventBus.AddListener<CustomizerUIEvents.SubmitClothingItemStart>(onSubmitItemStart);
			dependenciesSet = false;
			base.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			CatalogContext.EventBus.AddListener<CatalogModelEvents.CatalogStateChanged>(onChangeState);
			if (dependenciesSet)
			{
				changeState(CatalogState.Homepage);
			}
		}

		private void OnDisable()
		{
			CatalogContext.EventBus.RemoveListener<CatalogModelEvents.CatalogStateChanged>(onChangeState);
		}

		private void OnDestroy()
		{
			customizerEventBus.RemoveAllListeners();
		}

		public void SetDependencies(CatalogHomePageController homePageController, CatalogShopController shopController, CatalogHeaderController headerController, CatalogStatsPageController statsController, CatalogSubnavController subnavController)
		{
			this.homePageController = homePageController;
			this.shopController = shopController;
			this.headerController = headerController;
			this.statsController = statsController;
			this.subnavController = subnavController;
			dependenciesSet = true;
		}

		private bool onChangeState(CatalogModelEvents.CatalogStateChanged evt)
		{
			changeState(evt.State);
			return false;
		}

		private bool onSubmitItemStart(CustomizerUIEvents.SubmitClothingItemStart evt)
		{
			shopController.SubmissionCompletePanel.Init(evt.SubmittedItem);
			return false;
		}

		private void changeState(CatalogState state)
		{
			switch (state)
			{
			case CatalogState.Homepage:
				homePageController.Show();
				shopController.Hide();
				headerController.ShowStatsButton();
				statsController.Hide();
				subnavController.Hide();
				break;
			case CatalogState.ItemsView:
				homePageController.Show();
				homePageController.SetChallengeVisiblity(false);
				shopController.Show();
				headerController.ShowBackButton();
				statsController.Hide();
				subnavController.Show();
				break;
			case CatalogState.StatsView:
				homePageController.Hide();
				shopController.Hide();
				headerController.ShowBackButton();
				statsController.Show();
				subnavController.Hide();
				break;
			}
		}

		private void MarketplaceScreenIntroComplete()
		{
		}
	}
}
