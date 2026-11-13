using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;

namespace ClubPenguin.Igloo.UI
{
	public class IglooPlayUIStateController : AbstractIglooUIState
	{
		private IglooUIStateController stateController;

		private PrefabContentKey loadingScreenPrefab;

		public void Init(IglooUIStateController stateController)
		{
			this.stateController = stateController;
		}

		public override void OnEnter()
		{
			eventChannel.AddListener<IglooUIEvents.SetStateButtonPressed>(onSceneStateButton);
			eventChannel.AddListener<IglooUIEvents.CreateIglooButtonPressed>(onManageIglooCreatePressed);
			eventChannel.AddListener<IglooUIEvents.CloseManageIglooPopup>(onCloseManageIglooPopup);
			eventDispatcher.DispatchEvent(new PlayerSpawnedEvents.LocalPlayerReadyToSpawn(dataEntityCollection.LocalPlayerHandle));
			eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("QuestButton"));
		}

		public override void OnExit()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onCloseManageIglooPopup(IglooUIEvents.CloseManageIglooPopup evt)
		{
			stateController.ShowLoadingModalPopup();
			if (evt.SceneLayoutData != null)
			{
				onUpdateIglooDataFromCloseButton(true, evt.SceneLayoutData);
			}
			else
			{
				stateController.DataManager.UpdateIglooData(onUpdateIglooDataFromCloseButton);
			}
			return false;
		}

		private void onUpdateIglooDataFromCloseButton(bool success, SceneLayoutData sceneLayoutData)
		{
			stateController.HideLoadingModalPopup();
			if (success)
			{
				if (stateController.DataManager.LayoutManager.IsLayoutActive(sceneLayoutData.LayoutId))
				{
					stateController.CloseManageIglooPopup();
				}
				else
				{
					stateController.ReloadPlay(sceneLayoutData);
				}
			}
			else
			{
				stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, false);
				stateController.CloseManageIglooPopup();
			}
		}

		private bool onManageIglooCreatePressed(IglooUIEvents.CreateIglooButtonPressed evt)
		{
			stateController.ShowLoadingModalPopup();
			if (evt.SceneLayoutData != null)
			{
				onUpdateIglooDataFromCreateButton(true, evt.SceneLayoutData);
			}
			else
			{
				stateController.DataManager.UpdateIglooData(onUpdateIglooDataFromCreateButton);
			}
			return false;
		}

		private void onUpdateIglooDataFromCreateButton(bool success, SceneLayoutData sceneLayoutData)
		{
			stateController.HideLoadingModalPopup();
			if (!success)
			{
				stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, true);
				sceneLayoutData = stateController.DataManager.LayoutManager.GetActiveSceneLayoutData();
			}
			stateController.TransitionFromPlayToCreate(sceneLayoutData);
		}

		private bool onSceneStateButton(IglooUIEvents.SetStateButtonPressed evt)
		{
			SceneStateData.SceneState sceneState = evt.SceneState;
			if (sceneState == SceneStateData.SceneState.Edit)
			{
				setupEditState(evt.SplashScreen, evt.LayoutId);
			}
			return false;
		}

		private void setupEditState(PrefabContentKey loadingScreenPrefab, long layoutId)
		{
			this.loadingScreenPrefab = (loadingScreenPrefab ?? stateController.DefaultLoadingScreen);
			stateController.ShowLoadingModalPopup();
			stateController.DataManager.UpdateIglooData(delegate(bool success, SceneLayoutData sceneLayoutData)
			{
				if (!success)
				{
					stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, false);
				}
				stateController.DataManager.LoadIglooLayout(layoutId, onLayoutLoaded);
			});
		}

		private void onLayoutLoaded(bool success, SceneLayoutData publishedLayout, SceneLayoutData unpublishedLayout)
		{
			stateController.HideLoadingModalPopup();
			if (success)
			{
				SceneLayoutData sceneLayoutData = unpublishedLayout ?? publishedLayout;
				stateController.TransitionFromPlayToEdit(sceneLayoutData, loadingScreenPrefab);
			}
			else
			{
				Log.LogError(this, "Error loading the layout");
				stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, false);
			}
		}
	}
}
