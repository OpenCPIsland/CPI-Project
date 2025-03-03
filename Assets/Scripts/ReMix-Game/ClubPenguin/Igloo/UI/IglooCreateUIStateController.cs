using ClubPenguin.Analytics;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.Igloo.UI
{
	public class IglooCreateUIStateController : AbstractIglooUIState
	{
		private IglooUIStateController stateController;

		private PrefabContentKey loadingScreenPrefab;

		private LoadingController loadingController;

		public void Init(IglooUIStateController stateController)
		{
			this.stateController = stateController;
		}

		public override void OnEnter()
		{
			SuspendActiveQuest();
			if (stateController != null)
			{
				if (stateController.IsFirstIglooLoad && Service.Get<SceneLayoutDataManager>().IsInOwnIgloo())
				{
					stateController.OpenManageIglooPopup();
				}
				stateController.SendContextEvent("IglooScreenContainerContent", "igloolotsevent");
			}
			loadingController = Service.Get<LoadingController>();
			if (!loadingController.HasLoadingSystem(this))
			{
				loadingController.AddLoadingSystem(this);
			}
			eventChannel.AddListener<IglooUIEvents.SetStateButtonPressed>(onSceneStateButton);
			eventChannel.AddListener<IglooUIEvents.CreateIglooButtonPressed>(onManageIglooCreatePressed);
			eventChannel.AddListener<IglooUIEvents.CloseManageIglooPopup>(onCloseManageIglooPopup);
			eventChannel.AddListener<IglooUIEvents.LotNextButtonPressed>(onLotNextButtonPressed);
			eventChannel.AddListener<IglooUIEvents.LotBackButtonPressed>(onLotBackButtonPressed);
			eventChannel.AddListener<IglooUIEvents.LotScreenReady>(onLotScreenReady);
		}

		public override void OnExit()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onCloseManageIglooPopup(IglooUIEvents.CloseManageIglooPopup evt)
		{
			if (stateController.IsFirstIglooLoad)
			{
				stateController.CloseManageIglooPopup();
				stateController.ExitIgloos();
			}
			else
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
			}
			return false;
		}

		private void onUpdateIglooDataFromCloseButton(bool success, SceneLayoutData sceneLayoutData)
		{
			stateController.HideLoadingModalPopup();
			if (!success)
			{
				stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, true);
			}
			stateController.TransitionFromCreateToPlay(checkAndCacheSceneLayoutData(sceneLayoutData));
		}

		private bool onManageIglooCreatePressed(IglooUIEvents.CreateIglooButtonPressed evt)
		{
			if (!stateController.IsFirstIglooLoad)
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
			}
			stateController.CloseManageIglooPopup();
			SavedIgloosMetaData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				Service.Get<ICPSwrveService>().Action("igloo", "create", component.SavedIgloos.Count.ToString());
			}
			return false;
		}

		private bool onLotScreenReady(IglooUIEvents.LotScreenReady evt)
		{
			if (loadingController.HasLoadingSystem(this))
			{
				loadingController.RemoveLoadingSystem(this);
			}
			return false;
		}

		private void onUpdateIglooDataFromCreateButton(bool success, SceneLayoutData sceneLayoutData)
		{
			stateController.HideLoadingModalPopup();
			if (success)
			{
				checkAndCacheSceneLayoutData(sceneLayoutData);
			}
			else
			{
				stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, false);
			}
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
				if (unpublishedLayout != null)
				{
					stateController.TransitionFromCreateToEdit(unpublishedLayout, loadingScreenPrefab);
				}
				else
				{
					stateController.TransitionFromCreateToEdit(publishedLayout, loadingScreenPrefab);
				}
			}
			else
			{
				Log.LogError(this, "Error loading the layout");
				stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, false);
			}
		}

		private SceneLayoutData checkAndCacheSceneLayoutData(SceneLayoutData sceneLayoutData)
		{
			SceneLayoutData cachedSceneLayoutData = stateController.DataManager.LayoutManager.GetCachedSceneLayoutData();
			if (sceneLayoutData.LayoutId != cachedSceneLayoutData.LayoutId && sceneLayoutData.LayoutId > 0)
			{
				stateController.DataManager.LayoutManager.RemoveCachedSceneLayout();
				stateController.DataManager.LayoutManager.CacheLayoutFromSceneLayoutData(sceneLayoutData);
			}
			else
			{
				sceneLayoutData = cachedSceneLayoutData;
			}
			return sceneLayoutData;
		}

		private bool onLotBackButtonPressed(IglooUIEvents.LotBackButtonPressed evt)
		{
			stateController.OpenManageIglooPopup();
			return false;
		}

		private bool onLotNextButtonPressed(IglooUIEvents.LotNextButtonPressed evt)
		{
			stateController.ShowLoadingModalPopup();
			stateController.DataManager.CreateIgloo(onIglooCreated);
			return false;
		}

		private void onIglooCreated(bool success, SceneLayoutData sceneLayoutData)
		{
			stateController.HideLoadingModalPopup();
			if (success)
			{
				stateController.DataManager.LayoutManager.RemoveCachedSceneLayout();
				stateController.SetState(SceneStateData.SceneState.Edit);
				stateController.ResetUI();
			}
			else
			{
				eventChannel.AddListener<IglooUIEvents.ManageIglooPopupDisplayed>(onManageIglooPopupDisplayed);
				stateController.OpenManageIglooPopup();
			}
			Service.Get<EventDispatcher>().DispatchEvent(new IglooEvents.CreateIgloo(success));
		}

		private bool onManageIglooPopupDisplayed(IglooUIEvents.ManageIglooPopupDisplayed evt)
		{
			eventChannel.RemoveListener<IglooUIEvents.ManageIglooPopupDisplayed>(onManageIglooPopupDisplayed);
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Igloos.Menu.CreationError");
			Service.Get<EventDispatcher>().DispatchEvent(new IglooUIEvents.ShowNotification(tokenTranslation, 5f, null, true, false));
			return false;
		}
	}
}
