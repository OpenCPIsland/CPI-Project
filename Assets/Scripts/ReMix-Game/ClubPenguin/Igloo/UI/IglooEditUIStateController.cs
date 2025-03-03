using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;

namespace ClubPenguin.Igloo.UI
{
	public class IglooEditUIStateController : AbstractIglooUIState
	{
		private IglooUIStateController stateController;

		private DecorationInventoryService inventoryService;

		private bool isDecorationInventoryLoaded;

		private bool isStructureInventoryLoaded;

		private DataEventListener sceneLayoutListener;

		private SceneLayoutData publishedLayout;

		public void Init(IglooUIStateController stateController)
		{
			this.stateController = stateController;
		}

		protected override void Awake()
		{
			base.Awake();
			inventoryService = Service.Get<DecorationInventoryService>();
		}

		public void OnDestroy()
		{
		}

		public override void OnEnter()
		{
			sceneLayoutListener = dataEntityCollection.When<SceneLayoutData>(stateController.DataManager.LayoutManager.GetActiveHandle(), onSceneLayoutData);
			eventChannel.AddListener<IglooUIEvents.SetStateButtonPressed>(onSceneStateButton);
			SuspendActiveQuest();
			inventoryService.onInventoryLoaded += onDecorationsLoaded;
		}

		public override void OnExit()
		{
			if (sceneLayoutListener != null)
			{
				sceneLayoutListener.StopListening();
			}
			eventChannel.RemoveAllListeners();
			inventoryService.onInventoryLoaded -= onDecorationsLoaded;
		}

		private void onSceneLayoutData(SceneLayoutData sceneLayoutData)
		{
			if (stateController.DataManager.IsActiveIgloo(sceneLayoutData.LayoutId))
			{
				publishedLayout = stateController.DataManager.GetPublishedActiveLayout();
			}
			if (publishedLayout != null && !sceneLayoutData.IsSameLayout(publishedLayout))
			{
				PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("IglooConfirmUnfinishedBusinessPrompt");
				PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, onIglooConfirmUnfinishedBusinessPromptLoaded);
				promptLoaderCMD.Execute();
			}
			else
			{
				stateController.DataManager.StartSync();
			}
		}

		private void onIglooConfirmUnfinishedBusinessPromptLoaded(PromptLoaderCMD promptLoader)
		{
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onIglooConfirmUnfinishedBusinessPromptButtonClicked, promptLoader.Prefab);
		}

		private void onIglooConfirmUnfinishedBusinessPromptButtonClicked(DPrompt.ButtonFlags flags)
		{
			if (flags == DPrompt.ButtonFlags.NO)
			{
				stateController.ShowLoadingModalPopup();
				stateController.DataManager.LayoutManager.RemoveActiveSceneLayout();
				eventChannel.AddListener<SceneTransitionEvents.LayoutGameObjectsLoaded>(onSceneLayoutLoaded);
				stateController.DataManager.LayoutManager.AddNewSceneLayoutData(publishedLayout);
			}
			stateController.DataManager.StartSync();
		}

		private bool onSceneLayoutLoaded(SceneTransitionEvents.LayoutGameObjectsLoaded evt)
		{
			eventChannel.RemoveListener<SceneTransitionEvents.LayoutGameObjectsLoaded>(onSceneLayoutLoaded);
			stateController.HideLoadingModalPopup();
			return false;
		}

		private bool onSceneStateButton(IglooUIEvents.SetStateButtonPressed evt)
		{
			SceneLayoutData activeSceneLayoutData = stateController.DataManager.LayoutManager.GetActiveSceneLayoutData();
			switch (evt.SceneState)
			{
			case SceneStateData.SceneState.Play:
				if (activeSceneLayoutData.LayoutId == stateController.ActiveIglooId || stateController.IsFirstIglooLoad)
				{
					stateController.TransitionFromEditOrPreviewToPlay(true);
				}
				else
				{
					stateController.ShowConfirmPublishPrompt();
				}
				break;
			case SceneStateData.SceneState.Preview:
				stateController.TransitionFromEditToPreview();
				break;
			}
			return false;
		}

		private void onDecorationsLoaded(DecorationType decorationType)
		{
			switch (decorationType)
			{
			case DecorationType.Decoration:
				isDecorationInventoryLoaded = true;
				break;
			case DecorationType.Structure:
				isStructureInventoryLoaded = true;
				break;
			}
			if (!isDecorationInventoryLoaded || !isStructureInventoryLoaded)
			{
				return;
			}
			RecentDecorationsService recentDecorationsService = Service.Get<RecentDecorationsService>();
			if (recentDecorationsService.ShouldShowMostRecentPurchase)
			{
				string target = "IglooScreenContainerContent";
				string targetEvent = "igloonone";
				bool flag = false;
				if (recentDecorationsService.MostRecentPurchaseType == DecorationType.Decoration)
				{
					targetEvent = "igloofurnitureevent";
					flag = true;
				}
				else if (recentDecorationsService.MostRecentPurchaseType == DecorationType.Structure)
				{
					targetEvent = "igloostructuresevent";
					flag = true;
				}
				if (flag)
				{
					stateController.SendContextEvent(target, targetEvent);
				}
			}
		}
	}
}
