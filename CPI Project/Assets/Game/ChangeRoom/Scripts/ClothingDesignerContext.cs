using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner
{
	public class ClothingDesignerContext : MonoBehaviour
	{
		private static EventDispatcher eventBus;

		[SerializeField]
		private GameObject customizerAvatarPreview;

		[SerializeField]
		private GameObject screenContent;

		[SerializeField]
		private GameObject catalogContainer;

		[SerializeField]
		private RectTransform screenContainer;

		[SerializeField]
		private GameObject loadingIndicator;

		[SerializeField]
		private Texture2D[] defaultChannelTextures;

		private ClothingDesignerController controller;

		private BackButtonController backButtonController;

		private PresenceData localPresenceData;

		private bool isTutorialRunning = false;

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
			Service.Get<LoadingController>().AddLoadingSystem(this);
		}

		private void Start()
		{
			setPresenceData();
			controller = new ClothingDesignerController();
			controller.ControllerIsInitializing += onControllerReady;
			ClothingDesignerDependencies clothingDesignerDependencies = new ClothingDesignerDependencies();
			clothingDesignerDependencies.CustomizerAvatarPreview = customizerAvatarPreview;
			clothingDesignerDependencies.ScreenContent = screenContent;
			clothingDesignerDependencies.CatalogContainer = catalogContainer;
			clothingDesignerDependencies.ScreenContainer = screenContainer;
			clothingDesignerDependencies.LoadingIndicator = loadingIndicator;
			clothingDesignerDependencies.DefaultChannelTextures = defaultChannelTextures;
			controller.Init(clothingDesignerDependencies);
			GameStateController gameStateController = Service.Get<GameStateController>();
			if (!gameStateController.IsFTUEComplete && Service.Get<QuestService>().ActiveQuest.Id == gameStateController.FTUEConfig.FtueQuestId)
			{
				isTutorialRunning = true;
				backButtonController = Service.Get<BackButtonController>();
				if (backButtonController != null)
				{
					backButtonController.IsEnabled = false;
				}
			}
		}

		private void setPresenceData()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			localPresenceData = cPDataEntityCollection.GetComponent<PresenceData>(cPDataEntityCollection.LocalPlayerHandle);
			if (localPresenceData != null)
			{
				localPresenceData.IsNotInCurrentRoomsScene = true;
			}
			else
			{
				Log.LogError(this, "Unable to set IsNotInCurrentRoomsScene. Jump to friends may be broken.");
			}
		}

		private void onControllerReady()
		{
			Service.Get<LoadingController>().RemoveLoadingSystem(this);
		}

		private void OnDestroy()
		{
			if (localPresenceData != null)
			{
				localPresenceData.IsNotInCurrentRoomsScene = false;
			}
			if (isTutorialRunning && backButtonController != null)
			{
				backButtonController.IsEnabled = true;
			}
			controller.Destroy();
		}
	}
}
