using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.SceneManipulation;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	[RequireComponent(typeof(SceneSwapUIMediator))]
	[RequireComponent(typeof(StateMachineContextListener))]
	public class IglooUIStateController : AbstractIglooUIStateController
	{
		public enum IglooSaveStatus
		{
			None,
			ActiveIglooSaveSuccess,
			InActiveIglooSaveSuccess,
			Fail,
			GeneralError
		}

		private readonly PrefabContentKey LoadingPrefabKey = new PrefabContentKey("UI/IglooUIModalLoadingSpinner");

		public PrefabContentKey ManageIglooPopupKey;

		public PrefabContentKey DefaultLoadingScreen;

		public ZoneDefinition IglooExitZoneDefinition;

		[Header("UI Setup")]
		public SceneSwapUIMediator SceneSwapper;

		public StateMachineContextListener ContextListener;

		[Header("Data")]
		public IglooDataSyncManager DataManager;

		[Header("State Handlers")]
		public IglooCreateUIStateController CreateStateController;

		public IglooEditUIStateController EditStateController;

		public IglooPreviewUIStateController PreviewStateController;

		public IglooPlayUIStateController PlayStateController;

		private StateMachineContext context;

		private Queue<ExternalEvent> uiEventQueue;

		private bool hasStarted = false;

		private string previouslyPublishedLotName = null;

		private SceneStateData.SceneState previousState;

		private DataEventListener savedIglooListener;

		private SavedIgloosMetaData savedIgloosMetaData;

		private AbstractIglooUIState currentStateController;

		private bool isLoadingPopupActive;

		private GameObject loadingModal;

		private bool isManageIglooPopupActive = false;

		private GameObject manageIglooPopupPrefab;

		private GameObject manageIglooPopup;

		public StateMachineContext Context
		{
			get
			{
				return context;
			}
		}

		public SceneStateData.SceneState CurrentState
		{
			get
			{
				return sceneStateData.State;
			}
		}

		public SceneStateData.SceneState PreviousState
		{
			get
			{
				return previousState;
			}
		}

		public bool IsFirstIglooLoad
		{
			get
			{
				if (savedIgloosMetaData != null)
				{
					return savedIgloosMetaData.IsFirstIglooLoad();
				}
				return false;
			}
		}

		public long ActiveIglooId
		{
			get
			{
				if (savedIgloosMetaData != null && savedIgloosMetaData.ActiveIglooId.HasValue)
				{
					return savedIgloosMetaData.ActiveIglooId.Value;
				}
				return 0L;
			}
		}

		protected override void Awake()
		{
			CreateStateController.Init(this);
			EditStateController.Init(this);
			PreviewStateController.Init(this);
			PlayStateController.Init(this);
			uiEventQueue = new Queue<ExternalEvent>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			savedIglooListener = dataEntityCollection.When<SavedIgloosMetaData>(dataEntityCollection.LocalPlayerHandle, onSavedIgloosAdded);
			base.Awake();
			ContextListener.OnContextAdded += onContextAdded;
			eventChannel.AddListener<IglooUIEvents.OpenManageIglooPopup>(onOpenManageIglooPopup);
			ClubPenguin.Core.SceneRefs.Set(this);
		}

		private void Start()
		{
			if (sceneStateData != null)
			{
				onStateChanged(sceneStateData.State);
			}
			hasStarted = true;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (ContextListener != null)
			{
				ContextListener.OnContextAdded -= onContextAdded;
			}
			if (savedIglooListener != null)
			{
				savedIglooListener.StopListening();
			}
			ClubPenguin.Core.SceneRefs.Remove(this);
		}

		protected override void onSceneStateData(SceneStateData sceneStateData)
		{
			base.onSceneStateData(sceneStateData);
			if (hasStarted)
			{
				onStateChanged(sceneStateData.State);
			}
		}

		public override void StopListeningToStateChange()
		{
			base.StopListeningToStateChange();
			SceneStateToggler[] array = Object.FindObjectsOfType<SceneStateToggler>();
			SceneStateToggler[] array2 = array;
			foreach (SceneStateToggler sceneStateToggler in array2)
			{
				sceneStateToggler.StopListeningToStateChange();
			}
		}

		public void IglooSaveStatusNotification(IglooSaveStatus status, bool showAfterSceneLoad)
		{
			string text = null;
			switch (status)
			{
			case IglooSaveStatus.ActiveIglooSaveSuccess:
				text = Service.Get<Localizer>().GetTokenTranslation("Igloos.Menu.SaveConfirm");
				break;
			case IglooSaveStatus.InActiveIglooSaveSuccess:
				text = Service.Get<Localizer>().GetTokenTranslation("Igloos.Menu.SavedPublishHint");
				break;
			case IglooSaveStatus.Fail:
				text = Service.Get<Localizer>().GetTokenTranslation("Igloos.Menu.SaveError");
				break;
			case IglooSaveStatus.GeneralError:
				text = Service.Get<Localizer>().GetTokenTranslation("Igloos.Notification.GeneralError");
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				eventDispatcher.DispatchEvent(new IglooUIEvents.ShowNotification(text, 5f, null, false, showAfterSceneLoad));
			}
		}

		private void onContextAdded(StateMachineContext context)
		{
			ContextListener.OnContextAdded -= onContextAdded;
			this.context = context;
			dequeueContextEvents();
		}

		private void onSavedIgloosAdded(SavedIgloosMetaData savedIgloosMetaData)
		{
			this.savedIgloosMetaData = savedIgloosMetaData;
		}

		protected override void onStateChanged(SceneStateData.SceneState state)
		{
			if ((previousState == SceneStateData.SceneState.StructurePlacement && state == SceneStateData.SceneState.Edit) || (previousState == SceneStateData.SceneState.Edit && state == SceneStateData.SceneState.StructurePlacement) || (previousState == SceneStateData.SceneState.Edit && state == SceneStateData.SceneState.Edit) || state == SceneStateData.SceneState.StructurePlacement)
			{
				previousState = state;
				return;
			}
			if (currentStateController != null)
			{
				currentStateController.OnExit();
			}
			switch (state)
			{
			case SceneStateData.SceneState.Create:
				currentStateController = CreateStateController;
				if (previousState == SceneStateData.SceneState.Play)
				{
					DataManager.StartAFK();
				}
				break;
			case SceneStateData.SceneState.Play:
				currentStateController = PlayStateController;
				if (previousState == SceneStateData.SceneState.Edit)
				{
					DataManager.StopAFK();
				}
				break;
			case SceneStateData.SceneState.Edit:
				currentStateController = EditStateController;
				if (previousState == SceneStateData.SceneState.Play)
				{
					DataManager.StartAFK();
				}
				break;
			case SceneStateData.SceneState.Preview:
				currentStateController = PreviewStateController;
				break;
			default:
				currentStateController = PlayStateController;
				if (previousState == SceneStateData.SceneState.Edit)
				{
					DataManager.StopAFK();
				}
				break;
			}
			currentStateController.OnEnter();
			previousState = state;
		}

		private bool onOpenManageIglooPopup(IglooUIEvents.OpenManageIglooPopup evt)
		{
			OpenManageIglooPopup();
			Service.Get<ICPSwrveService>().Action("igloo", "manage");
			return false;
		}

		public void OpenManageIglooPopup()
		{
			if (!isManageIglooPopupActive)
			{
				if (CurrentState == SceneStateData.SceneState.Play)
				{
					SetPreviouslyPublishedLotName();
				}
				isManageIglooPopupActive = true;
				Content.LoadAsync(onManageIglooPopupLoaded, ManageIglooPopupKey);
			}
		}

		private void onManageIglooPopupLoaded(string path, GameObject prefab)
		{
			if (ClubPenguin.Core.SceneRefs.IsSet<PopupManager>())
			{
				manageIglooPopup = Object.Instantiate(prefab);
				eventDispatcher.DispatchEvent(new PopupEvents.ShowPopup(manageIglooPopup));
			}
			else
			{
				manageIglooPopupPrefab = prefab;
				eventChannel.AddListener<PopupEvents.PopupManagerReady>(onPopupManagerReady);
			}
		}

		private bool onPopupManagerReady(PopupEvents.PopupManagerReady evt)
		{
			eventChannel.RemoveListener<PopupEvents.PopupManagerReady>(onPopupManagerReady);
			manageIglooPopup = Object.Instantiate(manageIglooPopupPrefab);
			manageIglooPopupPrefab = null;
			eventDispatcher.DispatchEvent(new PopupEvents.ShowPopup(manageIglooPopup));
			return false;
		}

		public void CloseManageIglooPopup()
		{
			if (manageIglooPopup != null)
			{
				Object.Destroy(manageIglooPopup);
				isManageIglooPopupActive = false;
			}
		}

		public void ShowLoadingModalPopup()
		{
			if (!isLoadingPopupActive)
			{
				isLoadingPopupActive = true;
				Content.LoadAsync(onLoadingModalPopupLoaded, LoadingPrefabKey);
			}
		}

		private void onLoadingModalPopupLoaded(string path, GameObject obj)
		{
			if (isLoadingPopupActive)
			{
				loadingModal = Object.Instantiate(obj);
				eventDispatcher.DispatchEvent(new PopupEvents.ShowPopup(loadingModal));
			}
		}

		public void HideLoadingModalPopup()
		{
			if (isLoadingPopupActive)
			{
				if (loadingModal != null)
				{
					Object.Destroy(loadingModal);
				}
				isLoadingPopupActive = false;
			}
		}

		public void ReloadPlay(SceneLayoutData sceneLayoutData)
		{
			string lotZoneName = sceneLayoutData.LotZoneName;
			DataManager.LayoutManager.CacheLayoutFromSceneLayoutData(sceneLayoutData);
			DataManager.LayoutManager.RemoveActiveSceneLayout();
			CloseManageIglooPopup();
			ZoneId zoneId = new ZoneId();
			zoneId.name = lotZoneName;
			zoneId.instanceId = Service.Get<ZoneTransitionService>().CurrentInstanceId;
			Service.Get<ZoneTransitionService>().LoadIgloo(zoneId, Service.Get<Localizer>().Language, SceneStateData.SceneState.Play);
			previouslyPublishedLotName = null;
		}

		public void TransitionFromPlayToCreate(SceneLayoutData sceneLayoutData)
		{
			eventDispatcher.DispatchEvent(default(SceneTransitionEvents.SceneSwapLoadStarted));
			DataManager.LayoutManager.CacheLayoutFromSceneLayoutData(sceneLayoutData);
			DataManager.LayoutManager.RemoveActiveSceneLayout();
			CloseManageIglooPopup();
			StopListeningToStateChange();
			SetState(SceneStateData.SceneState.Create);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(SceneTransitionService.SceneArgs.LoadingScreenOverride.ToString(), DefaultLoadingScreen.Key);
			SceneSwapper.LoadScene("DefaultIgloo", dictionary);
		}

		public void TransitionFromPlayToEdit(SceneLayoutData sceneLayoutData, PrefabContentKey loadingScreenPrefab)
		{
			eventDispatcher.DispatchEvent(default(SceneTransitionEvents.SceneSwapLoadStarted));
			string sceneNameFromZoneName = IglooMediator.GetSceneNameFromZoneName(sceneLayoutData.LotZoneName);
			DataManager.LayoutManager.CacheLayoutFromSceneLayoutData(sceneLayoutData);
			DataManager.LayoutManager.RemoveActiveSceneLayout();
			CloseManageIglooPopup();
			StopListeningToStateChange();
			SetPreviouslyPublishedLotName();
			SetState(SceneStateData.SceneState.Edit);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(SceneTransitionService.SceneArgs.LoadingScreenOverride.ToString(), loadingScreenPrefab.Key);
			SceneSwapper.LoadScene(sceneNameFromZoneName, dictionary);
		}

		public void TransitionFromCreateToPlay(SceneLayoutData sceneLayoutData)
		{
			transitionToPlayFromAnyState(sceneLayoutData);
		}

		public void TransitionFromCreateToEdit(SceneLayoutData sceneLayoutData, PrefabContentKey loadingScreenPrefab)
		{
			string sceneNameFromZoneName = IglooMediator.GetSceneNameFromZoneName(sceneLayoutData.LotZoneName);
			if (DataManager.LayoutManager.HasCachedLayoutData())
			{
				DataManager.LayoutManager.RemoveCachedSceneLayout();
			}
			DataManager.LayoutManager.RemoveActiveSceneLayout();
			if (previouslyPublishedLotName != sceneLayoutData.LotZoneName)
			{
				ZoneId zoneId = new ZoneId();
				zoneId.name = sceneLayoutData.LotZoneName;
				zoneId.instanceId = Service.Get<ZoneTransitionService>().CurrentInstanceId;
				Service.Get<ZoneTransitionService>().LoadIgloo(zoneId, Service.Get<Localizer>().Language, SceneStateData.SceneState.Edit);
			}
			else
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(SceneTransitionService.SceneArgs.LoadingScreenOverride.ToString(), loadingScreenPrefab.Key);
				SceneSwapper.SwapScene(sceneNameFromZoneName, dictionary, delegate
				{
					SetState(SceneStateData.SceneState.Edit);
					ResetUI();
					DataManager.LayoutManager.AddNewSceneLayoutData(sceneLayoutData);
					CloseManageIglooPopup();
				});
			}
		}

		public void TransitionFromEditToPreview()
		{
			SetState(SceneStateData.SceneState.Preview);
		}

		public void TransitionFromPreviewToEdit()
		{
			SetState(SceneStateData.SceneState.Edit);
		}

		public void TransitionFromEditOrPreviewToPlay(bool publishLayout = false)
		{
			eventDispatcher.DispatchEvent(default(SceneTransitionEvents.SceneSwapLoadStarted));
			if (ClubPenguin.Core.SceneRefs.IsSet<SceneManipulationService>() && ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().ObjectManipulationInputController != null)
			{
				ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().ObjectManipulationInputController.Reset();
			}
			ShowLoadingModalPopup();
			if (publishLayout)
			{
				SetPreviouslyPublishedLotName();
				DataManager.UpdateAndPublishActiveLayout(onUpdateAndPublishActiveLayout);
			}
			else
			{
				DataManager.StopSync(onStopSyncReturnToPlay);
			}
		}

		private void onUpdateAndPublishActiveLayout(bool success, SceneLayoutData sceneLayoutData)
		{
			if (success)
			{
				if (sceneLayoutData.LayoutId == savedIgloosMetaData.ActiveIglooId)
				{
					IglooSaveStatusNotification(IglooSaveStatus.ActiveIglooSaveSuccess, true);
				}
				else
				{
					IglooSaveStatusNotification(IglooSaveStatus.InActiveIglooSaveSuccess, true);
				}
			}
			else
			{
				IglooSaveStatusNotification(IglooSaveStatus.Fail, true);
				sceneLayoutData = DataManager.LayoutManager.GetActiveSceneLayoutData();
			}
			HideLoadingModalPopup();
			transitionToPlayFromAnyState(sceneLayoutData);
		}

		private void onStopSyncReturnToPlay(bool success, SceneLayoutData sceneLayoutData)
		{
			if (success)
			{
				IglooSaveStatusNotification(IglooSaveStatus.InActiveIglooSaveSuccess, true);
			}
			else
			{
				IglooSaveStatusNotification(IglooSaveStatus.Fail, true);
			}
			HideLoadingModalPopup();
			if (sceneLayoutData.LayoutId == savedIgloosMetaData.ActiveIglooId)
			{
				transitionToPlayFromAnyState(sceneLayoutData);
			}
			else
			{
				DataManager.LoadIglooLayout(savedIgloosMetaData.ActiveIglooId.Value, onActiveLayoutLoaded);
			}
		}

		private void onActiveLayoutLoaded(bool success, SceneLayoutData publishedLayout, SceneLayoutData unpublishedLayout)
		{
			if (success)
			{
				transitionToPlayFromAnyState(publishedLayout);
				return;
			}
			Log.LogError(this, "Unknown Error. Cannot return back to the play state as expected");
			IglooSaveStatusNotification(IglooSaveStatus.GeneralError, true);
			ExitIgloos();
		}

		public void ShowConfirmPublishPrompt()
		{
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("IglooConfirmMakeActivePrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, onIglooConfirmMakeActivePromptLoaded);
			promptLoaderCMD.Execute();
		}

		private void onIglooConfirmMakeActivePromptLoaded(PromptLoaderCMD promptLoader)
		{
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onIglooConfirmMakeActivePromptButtonClicked, promptLoader.Prefab);
		}

		private void onIglooConfirmMakeActivePromptButtonClicked(DPrompt.ButtonFlags flags)
		{
			TransitionFromEditOrPreviewToPlay(flags == DPrompt.ButtonFlags.YES);
		}

		private void transitionToPlayFromAnyState(SceneLayoutData sceneLayoutData)
		{
			StopListeningToStateChange();
			SetState(SceneStateData.SceneState.Play);
			ReloadPlay(sceneLayoutData);
		}

		public void SetPreviouslyPublishedLotName()
		{
			foreach (SavedIglooMetaData savedIgloo in savedIgloosMetaData.SavedIgloos)
			{
				if (savedIgloo.LayoutId == savedIgloosMetaData.ActiveIglooId)
				{
					previouslyPublishedLotName = savedIgloo.LotZoneName;
				}
			}
		}

		public void ExitIgloos()
		{
			Service.Get<ZoneTransitionService>().LoadZone(IglooExitZoneDefinition);
		}

		public void SendContextEvent(string target, string targetEvent)
		{
			ExternalEvent externalEvent = new ExternalEvent(target, targetEvent);
			if (context == null)
			{
				uiEventQueue.Enqueue(externalEvent);
			}
			else
			{
				context.SendEvent(externalEvent);
			}
		}

		private void dequeueContextEvents()
		{
			if (uiEventQueue.Count > 1)
			{
			}
			while (uiEventQueue.Count > 0)
			{
				ExternalEvent evt = uiEventQueue.Dequeue();
				context.SendEvent(evt);
			}
		}

		public void ResetUI()
		{
			if (context != null)
			{
				SendContextEvent("IglooScreenContainerContent", "igloonone");
			}
		}
	}
}
