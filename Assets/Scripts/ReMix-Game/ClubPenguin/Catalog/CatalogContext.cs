using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Catalog
{
	public class CatalogContext : MonoBehaviour
	{
		private const CatalogState DEFAULT_STATE = CatalogState.Homepage;

		private static EventDispatcher eventBus;

		public CatalogViewController ViewController;

		public CatalogHomePageController HomePageController;

		public CatalogShopController ShopController;

		public CatalogHeaderController HeaderController;

		public CatalogStatsPageController StatsPageController;

		public CatalogSubnavController SubnavController;

		private CatalogModel mainModel;

		public static EventDispatcher EventBus
		{
			get
			{
				if (eventBus == null)
				{
					eventBus = new EventDispatcher();
				}
				return eventBus;
			}
		}

		private void Awake()
		{
			mainModel = new CatalogModel(CatalogState.Homepage);
		}

		private void Start()
		{
			init();
		}

		private void OnDestroy()
		{
			Service.Get<CatalogServiceProxy>().DeactivateCatalogTheme();
		}

		private void init()
		{
			HomePageController.SetModel(mainModel);
			ShopController.SetModel(mainModel);
			HeaderController.SetModel(mainModel);
			StatsPageController.SetModel(mainModel);
			SubnavController.SetModel(mainModel);
			ViewController.SetDependencies(HomePageController, ShopController, HeaderController, StatsPageController, SubnavController);
			EventBus.DispatchEvent(new CatalogModelEvents.CatalogStateChanged(CatalogState.Homepage));
		}
	}
}
