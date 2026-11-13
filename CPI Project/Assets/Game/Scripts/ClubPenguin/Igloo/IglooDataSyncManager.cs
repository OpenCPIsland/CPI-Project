using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Domain.Scene;
using ClubPenguin.SceneLayoutSync;
using ClubPenguin.SceneManipulation;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	public class IglooDataSyncManager : MonoBehaviour, IIglooCreateErrorHandler, IIglooUpdateLayoutErrorHandler, IIglooUpdateDataErrorHandler, IIglooUpdateAndPublishErrorHandler
	{
		private SceneLayoutDataManager layoutManager;

		private SceneLayoutSyncService syncService;

		private IIglooService iglooService;

		private DataEventListener savedIgloosListener;

		private SavedIgloosMetaData savedIgloosMetaData;

		private EventChannel eventChannel;

		private Action<bool, SceneLayoutData, SceneLayoutData> loadLayoutCallback;

		private Action<bool, SceneLayoutData> createCallback;

		private Action<bool, SceneLayoutData> updateAndPublishCallback;

		private Action<bool, SceneLayoutData> stopSyncCallback;

		private Action<bool, SceneLayoutData> updateDataCallback;

		public SceneLayoutDataManager LayoutManager
		{
			get
			{
				return layoutManager;
			}
		}

		private void Awake()
		{
			layoutManager = Service.Get<SceneLayoutDataManager>();
			syncService = Service.Get<SceneLayoutSyncService>();
			iglooService = Service.Get<INetworkServicesManager>().IglooService;
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			savedIgloosListener = Service.Get<CPDataEntityCollection>().When<SavedIgloosMetaData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle, onSavedIgloosMetaData);
		}

		private void OnDestroy()
		{
			if (savedIgloosListener != null)
			{
				savedIgloosListener.StopListening();
			}
			loadLayoutCallback = null;
			createCallback = null;
			updateAndPublishCallback = null;
			stopSyncCallback = null;
			updateDataCallback = null;
		}

		private void onSavedIgloosMetaData(SavedIgloosMetaData savedIgloosMetaData)
		{
			this.savedIgloosMetaData = savedIgloosMetaData;
		}

		public bool IsActiveIgloo(long layoutId)
		{
			return layoutId == savedIgloosMetaData.ActiveIglooId;
		}

		public void CreateIgloo(Action<bool, SceneLayoutData> callback)
		{
			createCallback = callback;
			SceneLayoutData activeSceneLayoutData = layoutManager.GetActiveSceneLayoutData();
			if (activeSceneLayoutData != null)
			{
				MutableSceneLayout mutableSceneLayout = new MutableSceneLayout();
				SceneLayoutSyncService.ConvertToMutableSceneLayout(mutableSceneLayout, activeSceneLayoutData);
				eventChannel.AddListener<IglooServiceEvents.IglooLayoutCreated>(onIglooCreated);
				iglooService.CreateIglooLayout(mutableSceneLayout, this);
				return;
			}
			Log.LogError(this, "Unable to create igloo. Scene layout data not in data model");
			if (createCallback != null)
			{
				createCallback.InvokeSafe(false, null);
				createCallback = null;
			}
		}

		private bool onIglooCreated(IglooServiceEvents.IglooLayoutCreated evt)
		{
			eventChannel.RemoveListener<IglooServiceEvents.IglooLayoutCreated>(onIglooCreated);
			foreach (SavedIglooMetaData savedIgloo in savedIgloosMetaData.SavedIgloos)
			{
				if (savedIgloo.LayoutId == evt.SavedSceneLayout.layoutId)
				{
					layoutManager.UpdateActiveLayoutFromData(evt.SavedSceneLayout.layoutId, evt.SavedSceneLayout);
					break;
				}
			}
			createCallback.InvokeSafe(true, LayoutManager.GetActiveSceneLayoutData());
			createCallback = null;
			return false;
		}

		public void LoadIglooLayout(long layoutId, Action<bool, SceneLayoutData, SceneLayoutData> callback)
		{
			loadLayoutCallback = callback;
			SceneLayout sceneLayout = null;
			if (layoutId != savedIgloosMetaData.ActiveIglooId)
			{
				for (int i = 0; i < savedIgloosMetaData.SavedIgloos.Count; i++)
				{
					if (savedIgloosMetaData.SavedIgloos[i].LayoutId == layoutId)
					{
						sceneLayout = savedIgloosMetaData.SavedIgloos[i].SceneLayout;
						break;
					}
				}
			}
			if (sceneLayout != null)
			{
				SceneLayoutData sceneLayoutData = new SceneLayoutData();
				layoutManager.UpdateSceneLayoutData(layoutId, sceneLayout, sceneLayoutData);
				if (loadLayoutCallback != null)
				{
					loadLayoutCallback.InvokeSafe(true, sceneLayoutData, null);
					loadLayoutCallback = null;
				}
			}
			else
			{
				eventChannel.AddListener<IglooServiceEvents.IglooLayoutLoaded>(onIglooLayoutLoaded);
				iglooService.GetIglooLayout(layoutId);
			}
		}

		private bool onIglooLayoutLoaded(IglooServiceEvents.IglooLayoutLoaded evt)
		{
			eventChannel.RemoveListener<IglooServiceEvents.IglooLayoutLoaded>(onIglooLayoutLoaded);
			SceneLayoutData arg = checkForUnpublishedLayout(evt.SavedSceneLayout.layoutId, evt.SavedSceneLayout);
			if (loadLayoutCallback != null)
			{
				SceneLayoutData sceneLayoutData = new SceneLayoutData();
				layoutManager.UpdateSceneLayoutData(evt.SavedSceneLayout.layoutId, evt.SavedSceneLayout, sceneLayoutData);
				loadLayoutCallback.InvokeSafe(true, sceneLayoutData, arg);
				loadLayoutCallback = null;
			}
			else
			{
				Log.LogError(this, "GetLayoutCallback was null");
			}
			return false;
		}

		private SceneLayoutData checkForUnpublishedLayout(long layoutId, SceneLayout sceneLayout)
		{
			SceneLayoutData sceneLayoutData = null;
			for (int i = 0; i < savedIgloosMetaData.SavedIgloos.Count; i++)
			{
				if (savedIgloosMetaData.SavedIgloos[i].LayoutId == layoutId && sceneLayout.lastModifiedDate > savedIgloosMetaData.SavedIgloos[i].LastModifiedDate)
				{
					sceneLayoutData = new SceneLayoutData();
					layoutManager.UpdateSceneLayoutData(layoutId, sceneLayout, sceneLayoutData);
					break;
				}
			}
			return sceneLayoutData;
		}

		public SceneLayoutData GetPublishedActiveLayout()
		{
			SceneLayoutData sceneLayoutData = new SceneLayoutData();
			for (int i = 0; i < savedIgloosMetaData.SavedIgloos.Count; i++)
			{
				if (layoutManager.IsLayoutActive(savedIgloosMetaData.SavedIgloos[i].LayoutId))
				{
					layoutManager.UpdateSceneLayoutData(savedIgloosMetaData.SavedIgloos[i].LayoutId, savedIgloosMetaData.SavedIgloos[i].SceneLayout, sceneLayoutData);
					break;
				}
			}
			return sceneLayoutData;
		}

		public void StartSync()
		{
			syncService.StartSyncingSceneLayoutData(layoutManager.GetActiveSceneLayoutData());
		}

		public void StopSync(Action<bool, SceneLayoutData> callback)
		{
			stopSyncCallback = callback;
			if (ClubPenguin.Core.SceneRefs.IsSet<SceneManipulationService>())
			{
				ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().ObjectManipulationInputController.Reset();
			}
			syncService.StopSyncingSceneLayoutData(layoutManager.GetActiveSceneLayoutData(), onSyncStopped, this);
		}

		private void onSyncStopped()
		{
			if (stopSyncCallback != null)
			{
				stopSyncCallback.InvokeSafe(true, layoutManager.GetActiveSceneLayoutData());
				stopSyncCallback = null;
			}
		}

		public void UpdateAndPublishActiveLayout(Action<bool, SceneLayoutData> callback = null)
		{
			updateAndPublishCallback = callback;
			SceneLayoutData activeSceneLayoutData = layoutManager.GetActiveSceneLayoutData();
			MutableSceneLayout mutableSceneLayout = new MutableSceneLayout();
			SceneLayoutSyncService.ConvertToMutableSceneLayout(mutableSceneLayout, activeSceneLayoutData);
			IglooVisibility? visibility = null;
			if (savedIgloosMetaData.IsDirty)
			{
				visibility = savedIgloosMetaData.IglooVisibility;
			}
			if (updateAndPublishCallback != null)
			{
				eventChannel.AddListener<IglooServiceEvents.IglooPublished>(onIglooPublished);
			}
			iglooService.UpdateAndPublish(activeSceneLayoutData.LayoutId, visibility, mutableSceneLayout, this);
			logBIForSavingIglooLayout(activeSceneLayoutData);
		}

		private bool onIglooPublished(IglooServiceEvents.IglooPublished evt)
		{
			eventChannel.RemoveListener<IglooServiceEvents.IglooPublished>(onIglooPublished);
			layoutManager.UpdateActiveLayoutFromData(evt.SavedSceneLayout.layoutId, evt.SavedSceneLayout);
			if (updateAndPublishCallback != null)
			{
				updateAndPublishCallback.InvokeSafe(true, layoutManager.GetActiveSceneLayoutData());
				updateAndPublishCallback = null;
			}
			return false;
		}

		public void UpdateIglooData(Action<bool, SceneLayoutData> callback = null)
		{
			updateDataCallback = callback;
			if (savedIgloosMetaData.IsDirty)
			{
				eventChannel.AddListener<IglooServiceEvents.IglooDataUpdated>(onIglooDataUpdated);
				iglooService.UpdateIglooData(savedIgloosMetaData.IglooVisibility, savedIgloosMetaData.ActiveIglooId, this);
				return;
			}
			logBIForSavingIglooLayout(layoutManager.GetActiveSceneLayoutData());
			if (updateDataCallback != null)
			{
				updateDataCallback.InvokeSafe(true, layoutManager.GetActiveSceneLayoutData());
				updateDataCallback = null;
			}
		}

		private bool onIglooDataUpdated(IglooServiceEvents.IglooDataUpdated evt)
		{
			eventChannel.RemoveListener<IglooServiceEvents.IglooDataUpdated>(onIglooDataUpdated);
			SceneLayoutData sceneLayoutData = new SceneLayoutData();
			if (evt.SignedIglooData.Data.activeLayoutId.HasValue)
			{
				layoutManager.UpdateSceneLayoutData(evt.SignedIglooData.Data.activeLayoutId.Value, evt.SignedIglooData.Data.activeLayout, sceneLayoutData);
			}
			else
			{
				Log.LogError(this, "Null layoutId returned, it's possible membership exipired at this point");
				if (layoutManager.HasCachedLayoutData())
				{
					sceneLayoutData = layoutManager.GetCachedSceneLayoutData();
				}
			}
			logBIForSavingIglooLayout(sceneLayoutData);
			if (updateDataCallback != null)
			{
				updateDataCallback.InvokeSafe(true, sceneLayoutData);
				updateDataCallback = null;
			}
			return false;
		}

		public void UpdateIglooDataAndDeleteLayout()
		{
		}

		public void StartAFK()
		{
			Service.Get<INetworkServicesManager>().PlayerStateService.SetAwayFromKeyboard(7);
		}

		public void StopAFK()
		{
			Service.Get<INetworkServicesManager>().PlayerStateService.SetAwayFromKeyboard(0);
		}

		private void logBIForSavingIglooLayout(SceneLayoutData sceneLayoutData)
		{
			if (sceneLayoutData != null)
			{
				Service.Get<ICPSwrveService>().Action("igloo", "save", sceneLayoutData.LayoutCount.ToString());
			}
		}

		public void OnCreateLayoutError()
		{
			Log.LogError(this, "OnCreateLayoutError");
			eventChannel.RemoveListener<IglooServiceEvents.IglooLayoutCreated>(onIglooCreated);
			if (createCallback != null)
			{
				createCallback.InvokeSafe(false, null);
				createCallback = null;
			}
		}

		public void OnUpdateLayoutError()
		{
			Log.LogError(this, "OnUpdateLayoutError");
			eventChannel.RemoveListener<IglooServiceEvents.IglooPublished>(onIglooPublished);
			if (updateAndPublishCallback != null)
			{
				updateAndPublishCallback.InvokeSafe(false, null);
				updateAndPublishCallback = null;
			}
			if (stopSyncCallback != null)
			{
				stopSyncCallback.InvokeSafe(false, null);
				stopSyncCallback = null;
			}
		}

		public void OnUpdateDataError()
		{
			Log.LogError(this, "OnUpdateDataError");
			eventChannel.RemoveListener<IglooServiceEvents.IglooDataUpdated>(onIglooDataUpdated);
			if (updateDataCallback != null)
			{
				updateDataCallback.InvokeSafe(false, null);
				updateDataCallback = null;
			}
		}

		public void OnUpdateAndPublishError()
		{
			Log.LogError(this, "OnUpdateAndPublishError");
			if (updateAndPublishCallback != null)
			{
				updateAndPublishCallback.InvokeSafe(false, null);
			}
		}
	}
}
