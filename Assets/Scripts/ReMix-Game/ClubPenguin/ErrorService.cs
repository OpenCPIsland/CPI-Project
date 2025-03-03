using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class ErrorService
	{
		public enum RoomJoinErrorType
		{
			RoomJoinErrorEvent,
			RoomJoinGeneralNetworkError,
			RoomJoinRequestTimeOut,
			RoomJoinNoRoomsError
		}

		private struct ErrorData
		{
			public string TitleToken;

			public string MessageToken;

			public Action<DPrompt.ButtonFlags> Callback;

			public PromptController PromptPrefab;

			public DPrompt.ButtonFlags Buttons;

			public ErrorData(string titleToken, string messageToken, Action<DPrompt.ButtonFlags> callback = null)
			{
				TitleToken = titleToken;
				MessageToken = messageToken;
				Callback = callback;
				PromptPrefab = null;
				Buttons = DPrompt.ButtonFlags.None;
			}
		}

		public const string NETWORK_ERROR_TITLE_TOKEN = "GlobalUI.ErrorMessages.NetworkError.Title";

		public const string NETWORK_ERROR_TOKEN = "GlobalUI.ErrorMessages.CheckNetworkConnection";

		public const string WORLD_DISCONNECTED_ERROR_TOKEN = "GlobalUI.ErrorMessages.WorldDisconnected";

		public const string WORLD_NETWORK_ERROR_TOKEN = "GlobalUI.ErrorMessages.WorldNetworkError";

		public const string ROOM_JOIN_ERROR_TOKEN = "GlobalUI.ErrorMessages.RoomJoinError";

		public const string ROOM_LEAVE_ERROR_TOKEN = "GlobalUI.ErrorMessages.RoomLeaveError";

		public const string RETRY_FAILURE_TOKEN = "GlobalUI.ErrorMessages.RetryFailureError";

		public const string SESSION_ERROR_SESSION_TERMINATED_TITLE_TOKEN = "GlobalUI.ErrorMessages.SessionLostError.Title";

		public const string SESSION_ERROR_SESSION_TERMINATED_TOKEN = "GlobalUI.ErrorMessages.SessionLostError";

		public const string SESSION_ERROR_SESSION_CORRUPTED_TITLE_TOKEN = "GlobalUI.ErrorMessages.SessionDataCorruptedError.Title";

		public const string SESSION_ERROR_SESSION_CORRUPTED_TOKEN = "GlobalUI.ErrorMessages.SessionDataCorruptedError";

		public const string SESSION_ERROR_SESSION_CORRUPTED_UNRECOVERABLE_TITLE_TOKEN = "GlobalUI.ErrorMessages.SessionDataCorruptedUnrecoverableError.Title";

		public const string SESSION_ERROR_SESSION_CORRUPTED_UNRECOVERABLE_TOKEN = "GlobalUI.ErrorMessages.SessionDataCorruptedUnrecoverableError";

		public const string REGISTRATION_CONFIG_ERROR_TITLE_TOKEN = "Account.Create.Error.OneID.Title";

		public const string REGISTRATION_CONFIG_ERROR_TOKEN = "Account.Create.Error.OneID";

		public const string BANNED_TITLE_TOKEN = "GlobalUI.ErrorMessages.AccountBanned.Title";

		public const string AUTHENTICATION_ERROR_TITLE_TOKEN = "GlobalUI.ErrorMessages.AuthenticationLost.Title";

		public const string AUTHENTICATION_ERROR_TEXT_TOKEN = "GlobalUI.ErrorMessages.AuthenticationLost";

		public const string PARENTAL_CONSENT_TITLE_TOKEN = "GlobalUI.ErrorMessages.ParentalConsent.Title";

		public const string PARENTAL_CONSENT_TEXT_TOKEN = "GlobalUI.ErrorMessages.ParentalConsent";

		private const int MAX_ERRORS = 2;

		private const int errorCount = 0;

		private const int MAX_RECONNECTION_ATTEMPTS = 0;

		private const int SERVER_LIMIT = 5;

		private bool showingError;

		private ConnectionManager connectionManager;

		private GameStateController gameStateController;

		private PromptManager promptManager;

		private SessionManager sessionManager;

		private CPDataEntityCollection dataEntityCollection;

		private ZoneTransitionService zoneTransitionService;

		private EventDispatcher eventDispatcher;

		private readonly SpriteContentKey parentalConsentIconContentKey = new SpriteContentKey("Images/SharedAssets_ErrorMessages_ParentalConsent");

		private readonly SpriteContentKey ApologiesIconContentKey = new SpriteContentKey("Images/SharedAssets_ErrorMessages_Apologies");

		private readonly SpriteContentKey SessionLostIconContentKey = new SpriteContentKey("Images/SharedAssets_ErrorMessages_SessionLost");

		private readonly SpriteContentKey NetworkErrorIconContentKey = new SpriteContentKey("Images/SharedAssets_ErrorMessages_NetworkError");

		private Dictionary<string, Sprite> iconSprites = new Dictionary<string, Sprite>();

		private int reconnectionAttempts = 0;

		public ErrorService()
		{
			connectionManager = Service.Get<ConnectionManager>();
			gameStateController = Service.Get<GameStateController>();
			promptManager = Service.Get<PromptManager>();
			sessionManager = Service.Get<SessionManager>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			zoneTransitionService = Service.Get<ZoneTransitionService>();
			eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<ApplicationService.Error>(onError);
			eventDispatcher.AddListener<SessionErrorEvents.AccountBannedEvent>(onAccountBannedEvent);
			eventDispatcher.AddListener<SessionErrorEvents.AuthenticationRequiresParentalConsentEvent>(onAuthenticationRequiresParentalConsentEvent);
			eventDispatcher.AddListener<SessionErrorEvents.AuthenticationRevokedEvent>(onAuthenticationRevokedEvent);
			eventDispatcher.AddListener<SessionErrorEvents.AuthenticationUnavailableEvent>(onAuthenticationUnavailableEvent);
			eventDispatcher.AddListener<SessionErrorEvents.SessionTerminated>(onSessionTerminated);
			eventDispatcher.AddListener<SessionErrorEvents.SessionDataCorrupted>(onSessionDataCorrupted);
			eventDispatcher.AddListener<SessionErrorEvents.NoNetworkOnResumeError>(onNoNetworkOnResume);
			eventDispatcher.AddListener<SessionErrorEvents.NoSessionOnResumeError>(onNoSessionOnResume);
			eventDispatcher.AddListener<SessionErrorEvents.RegistrationConfigError>(onRegistrationConfigError);
			eventDispatcher.AddListener<WorldServiceErrors.WorldDisconnectedEvent>(onWorldDisconnected);
			eventDispatcher.AddListener<WorldServiceErrors.WorldNetworkErrorEvent>(onWorldNetworkError);
			eventDispatcher.AddListener<NetworkErrors.NoConnectionError>(onNoConnectionError);
			eventDispatcher.AddListener<NetworkErrors.GeneralError>(onGeneralNetworkError);
			Service.Get<EventDispatcher>().AddListener<LoadingController.TimeoutEvent>(onLoadingScreenTimeout);
			eventDispatcher.AddListener<WorldServiceEvents.SelfRoomJoinedEvent>(onRoomJoined);
			CoroutineRunner.StartPersistent(preloadIcons(), this, "Preload error icons");
		}

		private IEnumerator preloadIcons()
		{
			AssetRequest<Sprite> request4 = Content.LoadAsync(parentalConsentIconContentKey);
			yield return request4;
			iconSprites.Add(parentalConsentIconContentKey.Key, request4.Asset);
			request4 = Content.LoadAsync(ApologiesIconContentKey);
			yield return request4;
			iconSprites.Add(ApologiesIconContentKey.Key, request4.Asset);
			request4 = Content.LoadAsync(SessionLostIconContentKey);
			yield return request4;
			iconSprites.Add(SessionLostIconContentKey.Key, request4.Asset);
			request4 = Content.LoadAsync(NetworkErrorIconContentKey);
			yield return request4;
			iconSprites.Add(NetworkErrorIconContentKey.Key, request4.Asset);
		}

		private bool onError(ApplicationService.Error evt)
		{
			Service.Get<ICPSwrveService>().Error("error_prompt", evt.Type, SceneManager.GetActiveScene().name);
			showErrorPrompt(evt.Type, evt.Message);
			return false;
		}

		private bool onAccountBannedEvent(SessionErrorEvents.AccountBannedEvent evt)
		{
			Service.Get<ICPSwrveService>().Error("error_prompt", "AccountBannedEvent", SceneManager.GetActiveScene().name);
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("ModerationCriticalPrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, delegate(PromptLoaderCMD loader)
			{
				showAccountBannedPrompt(loader, evt);
			});
			promptLoaderCMD.Execute();
			return true;
		}

		private void showAccountBannedPrompt(PromptLoaderCMD promptLoader, SessionErrorEvents.AccountBannedEvent evt)
		{
			showingError = true;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DisplayNameData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				promptLoader.PromptData.SetText(DPrompt.PROMPT_TEXT_TITLE, component.DisplayName, true);
			}
			else
			{
				promptLoader.PromptData.SetText(DPrompt.PROMPT_TEXT_TITLE, "GlobalUI.ErrorMessages.AccountBanned.Title");
			}
			SessionErrorEvents.AccountBannedPromptSetup(promptLoader.PromptData, evt.Category, evt.ExpirationDate);
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, clearShowingError, promptLoader.Prefab);
			gameStateController.ExitAfterBan();
		}

		private bool onAuthenticationRequiresParentalConsentEvent(SessionErrorEvents.AuthenticationRequiresParentalConsentEvent evt)
		{
			Service.Get<ICPSwrveService>().Error("error_prompt", "AuthenticationRequiresParentalConsentEvent", SceneManager.GetActiveScene().name);
			ErrorData errorData = new ErrorData("GlobalUI.ErrorMessages.ParentalConsent.Title", "GlobalUI.ErrorMessages.ParentalConsent");
			showIconErrorPrompt(errorData, parentalConsentIconContentKey);
			gameStateController.ExitWorld(true);
			return true;
		}

		private bool onAuthenticationRevokedEvent(SessionErrorEvents.AuthenticationRevokedEvent evt)
		{
			Service.Get<ICPSwrveService>().Error("error_prompt", "AuthenticationRevokedEvent", SceneManager.GetActiveScene().name);
			promptManager.ShowPrompt("AuthenticationErrorPrompt", null);
			gameStateController.ExitWorld(true);
			return true;
		}

		private bool onAuthenticationUnavailableEvent(SessionErrorEvents.AuthenticationUnavailableEvent evt)
		{
			Service.Get<ICPSwrveService>().Error("error_prompt", "AuthenticationUnavailableEvent", SceneManager.GetActiveScene().name);
			showErrorPrompt("GlobalUI.ErrorMessages.AuthenticationLost.Title", "GlobalUI.ErrorMessages.AuthenticationLost");
			gameStateController.ExitWorld(true);
			return true;
		}

		private bool onSessionTerminated(SessionErrorEvents.SessionTerminated evt)
		{
			Service.Get<ICPSwrveService>().Error("error_prompt", "SessionTerminated", SceneManager.GetActiveScene().name);
			ErrorData errorData = new ErrorData("GlobalUI.ErrorMessages.SessionLostError.Title", "GlobalUI.ErrorMessages.SessionLostError");
			showIconErrorPrompt(errorData, SessionLostIconContentKey);
			gameStateController.ExitWorld(true);
			return true;
		}

		private bool onSessionDataCorrupted(SessionErrorEvents.SessionDataCorrupted evt)
		{
			Service.Get<ICPSwrveService>().Error("error_prompt", "SessionDataCorrupted", SceneManager.GetActiveScene().name);
			string titleToken = "GlobalUI.ErrorMessages.SessionDataCorruptedError.Title";
			string messageToken = "GlobalUI.ErrorMessages.SessionDataCorruptedError";
			if (!evt.Recovered)
			{
				titleToken = "GlobalUI.ErrorMessages.SessionDataCorruptedUnrecoverableError.Title";
				messageToken = "GlobalUI.ErrorMessages.SessionDataCorruptedUnrecoverableError";
			}
			ErrorData errorData = new ErrorData(titleToken, messageToken);
			showIconErrorPrompt(errorData, SessionLostIconContentKey);
			gameStateController.ExitWorld(true);
			return true;
		}

		private bool onLoadingScreenTimeout(LoadingController.TimeoutEvent evt)
		{
			Service.Get<ICPSwrveService>().Error("error_prompt", "TimeoutEvent", SceneManager.GetActiveScene().name);
			Log.LogErrorFormatted(this, "Transition Error: Timeout {0}, {1}", gameStateController.CurrentState(), evt.ElapsedTime);
			promptManager.ShowPrompt("UnknownErrorPrompt", null);
			gameStateController.ExitWorld();
			return false;
		}

		private bool onRegistrationConfigError(SessionErrorEvents.RegistrationConfigError evt)
		{
			Service.Get<ICPSwrveService>().Error("error_prompt", "RegConfigError", SceneManager.GetActiveScene().name);
			MixLoginCreateService loginService = Service.Get<MixLoginCreateService>();
			ErrorData errorData = new ErrorData(evt.TitleToken, evt.MessageToken);
			errorData.PromptPrefab = promptManager.ErrorPrefab;
			errorData.Buttons = DPrompt.ButtonFlags.OK;
			Service.Get<ConnectionManager>().DoPingCheck(delegate(ConnectionManager.NetworkConnectionState connectionState)
			{
				if (connectionState == ConnectionManager.NetworkConnectionState.NoConnection)
				{
					loginService.RegConfigAttempts = 0;
					if (!showingError)
					{
						showNetworkErrorPrompt("GlobalUI.ErrorMessages.NetworkError.Title", "GlobalUI.ErrorMessages.WorldNetworkError");
					}
				}
				else
				{
					if (!Service.Get<MixLoginCreateService>().IsFetchingRegConfig)
					{
						loginService.GetRegistrationConfig();
					}
					showIconPrompt(errorData, NetworkErrorIconContentKey);
					Log.LogErrorFormatted(this, "Registration Config Error. Step: {0}", evt.Step);
				}
			});
			return false;
		}

		private void showIconErrorPrompt(ErrorData errorData, SpriteContentKey iconContentKey)
		{
			errorData.PromptPrefab = promptManager.ErrorPrefab;
			errorData.Buttons = DPrompt.ButtonFlags.OK;
			showingError = true;
			showIconPrompt(errorData, iconContentKey);
		}

		private void showIconPrompt(ErrorData errorData, SpriteContentKey iconContentKey)
		{
			DPrompt data = new DPrompt(errorData.TitleToken, errorData.MessageToken, errorData.Buttons, iconSprites[iconContentKey.Key]);
			promptManager.ShowPrompt(data, errorData.Callback ?? new Action<DPrompt.ButtonFlags>(clearShowingError), errorData.PromptPrefab);
		}

		private void showErrorPrompt(string titleToken, string messageToken)
		{
			showingError = true;
			promptManager.ShowError(titleToken, messageToken, clearShowingError);
		}

		private void clearShowingError(DPrompt.ButtonFlags flags)
		{
			showingError = false;
		}

		private void clearData()
		{
			dataEntityCollection.ClearZoneScope();
		}

		private bool onGeneralNetworkError(NetworkErrors.GeneralError evt)
		{
			Service.Get<ICPSwrveService>().Error("network_error", "GeneralError", gameStateController.CurrentState(), SceneManager.GetActiveScene().name, evt.Error.code.ToString());
			Log.LogNetworkErrorFormatted(this, "GeneralError: {0}, {1}", gameStateController.CurrentState(), evt.Error.code);
			return false;
		}

		private bool onNoConnectionError(NetworkErrors.NoConnectionError evt)
		{
			if (showingError)
			{
				return false;
			}
			Service.Get<ICPSwrveService>().Error("network_error", "NoConnectionError", gameStateController.CurrentState(), SceneManager.GetActiveScene().name);
			Log.LogNetworkErrorFormatted(this, "No Connection state: {0}", gameStateController.CurrentState());
			clearData();
			gameStateController.GoOffline();
			showNetworkErrorPrompt("GlobalUI.ErrorMessages.NetworkError.Title", "GlobalUI.ErrorMessages.CheckNetworkConnection");
			return false;
		}

		private bool onWorldDisconnected(WorldServiceErrors.WorldDisconnectedEvent evt)
		{
			if (showingError)
			{
				return false;
			}
			Service.Get<ICPSwrveService>().Error("network_error", "WorldDisconnectedEvent", gameStateController.CurrentState(), SceneManager.GetActiveScene().name);
			clearData();
			gameStateController.GoOffline();
			showNetworkErrorPrompt("GlobalUI.ErrorMessages.NetworkError.Title", "GlobalUI.ErrorMessages.WorldDisconnected");
			if (Service.Get<ConnectionManager>().ConnectionState == ConnectionManager.NetworkConnectionState.NoConnection)
			{
				Log.LogNetworkErrorFormatted(this, "World Disconnect connection status: NoConnection state: {0}", gameStateController.CurrentState());
			}
			else
			{
				Service.Get<ConnectionManager>().DoPingCheck(delegate(ConnectionManager.NetworkConnectionState connectionState)
				{
					Log.LogNetworkErrorFormatted(this, "World Disconnect connection status: {0} state: {1}", connectionState.ToString(), gameStateController.CurrentState());
				});
			}
			return false;
		}

		private bool onWorldNetworkError(WorldServiceErrors.WorldNetworkErrorEvent evt)
		{
			if (showingError)
			{
				return false;
			}
			Service.Get<ICPSwrveService>().Error("network_error", "WorldNetworkErrorEvent", gameStateController.CurrentState(), SceneManager.GetActiveScene().name);
			Log.LogNetworkErrorFormatted(this, "World Network Error state: {0}", gameStateController.CurrentState());
			clearData();
			gameStateController.GoOffline();
			showNetworkErrorPrompt("GlobalUI.ErrorMessages.NetworkError.Title", "GlobalUI.ErrorMessages.WorldNetworkError");
			return false;
		}

		public void OnRoomJoinError(RoomJoinErrorType errorType)
		{
			if (!showingError)
			{
				Service.Get<ICPSwrveService>().Error("network_error", errorType.ToString(), gameStateController.CurrentState(), SceneManager.GetActiveScene().name);
				Log.LogNetworkErrorFormatted(this, "Room Join Error, type: {0} state: {1}", errorType.ToString(), gameStateController.CurrentState());
				clearData();
				gameStateController.GoOffline();
				showNetworkErrorPrompt("GlobalUI.ErrorMessages.NetworkError.Title", "GlobalUI.ErrorMessages.RoomJoinError", onRoomJoinButtonPressed, 3);
			}
		}

		public bool OnNoServersError()
		{
			if (showingError)
			{
				return false;
			}
			Service.Get<ICPSwrveService>().Error("network_error", "NoServersError", gameStateController.CurrentState(), SceneManager.GetActiveScene().name);
			Log.LogNetworkErrorFormatted(this, "No Servers state: {0}", gameStateController.CurrentState());
			ErrorData errorData = new ErrorData("GlobalUI.ErrorMessages.NetworkError.Title", "GlobalUI.ErrorMessages.SessionLostError");
			showIconErrorPrompt(errorData, SessionLostIconContentKey);
			gameStateController.ExitWorld();
			return false;
		}

		public bool onNoNetworkOnResume(SessionErrorEvents.NoNetworkOnResumeError evt)
		{
			if (showingError)
			{
				return false;
			}
			Service.Get<ICPSwrveService>().Error("network_error", "NoNetworkOnResumeError", gameStateController.CurrentState(), SceneManager.GetActiveScene().name);
			Log.LogNetworkErrorFormatted(this, "No Network On Resume state: {0}", gameStateController.CurrentState());
			ErrorData errorData = new ErrorData("GlobalUI.ErrorMessages.NetworkError.Title", "GlobalUI.ErrorMessages.SessionLostError");
			showIconErrorPrompt(errorData, SessionLostIconContentKey);
			gameStateController.ExitWorld();
			return false;
		}

		public bool onNoSessionOnResume(SessionErrorEvents.NoSessionOnResumeError evt)
		{
			if (showingError)
			{
				return false;
			}
			Service.Get<ICPSwrveService>().Error("network_error", "NoSessionOnResumeError", gameStateController.CurrentState(), SceneManager.GetActiveScene().name);
			Log.LogNetworkErrorFormatted(this, "No Session on Resume state: {0}", gameStateController.CurrentState());
			ErrorData errorData = new ErrorData("GlobalUI.ErrorMessages.NetworkError.Title", "GlobalUI.ErrorMessages.SessionLostError");
			showIconErrorPrompt(errorData, SessionLostIconContentKey);
			gameStateController.ExitWorld();
			return false;
		}

		private void showNetworkErrorPrompt(string titleToken, string messageToken, Action<DPrompt.ButtonFlags> callback = null, int maxReconnectionOverride = 0)
		{
			showingError = true;
			zoneTransitionService.CancelTransition(gameStateController.SceneConfig.HomeSceneName);
			ErrorData errorData = new ErrorData(titleToken, messageToken);
			errorData.Buttons = (DPrompt.ButtonFlags.CANCEL | DPrompt.ButtonFlags.RETRY);
			errorData.PromptPrefab = promptManager.ErrorPrefab;
			errorData.Callback = (callback ?? new Action<DPrompt.ButtonFlags>(onButtonPressed));
			if (connectionManager.ConnectionState == ConnectionManager.NetworkConnectionState.BasicConnection && sessionManager.HasSession)
			{
				int num = 0;
				if (maxReconnectionOverride > 0)
				{
					num = maxReconnectionOverride;
				}
				if (reconnectionAttempts++ >= num)
				{
					errorData.MessageToken = "GlobalUI.ErrorMessages.RetryFailureError";
					errorData.Buttons = DPrompt.ButtonFlags.OK;
					errorData.PromptPrefab = promptManager.FatalPrefab;
					reconnectionAttempts = 0;
				}
			}
			else
			{
				errorData.MessageToken = messageToken;
				errorData.Buttons = DPrompt.ButtonFlags.OK;
				errorData.PromptPrefab = promptManager.FatalPrefab;
				reconnectionAttempts = 0;
			}
			showIconPrompt(errorData, NetworkErrorIconContentKey);
		}

		private void onButtonPressed(DPrompt.ButtonFlags buttonFlag)
		{
			showingError = false;
			int num;
			switch (buttonFlag)
			{
			case DPrompt.ButtonFlags.RETRY:
				gameStateController.RetryConnection(Service.Get<ZoneTransitionService>().CurrentZone.ZoneName);
				return;
			default:
				num = ((buttonFlag != DPrompt.ButtonFlags.OK) ? 1 : 0);
				break;
			case DPrompt.ButtonFlags.CANCEL:
				num = 0;
				break;
			}
			if (num == 0)
			{
				gameStateController.ExitWorld();
			}
		}

		private void onRoomJoinButtonPressed(DPrompt.ButtonFlags buttonFlag)
		{
			showingError = false;
			int num;
			switch (buttonFlag)
			{
			case DPrompt.ButtonFlags.RETRY:
				if (connectionManager.ConnectionState == ConnectionManager.NetworkConnectionState.BasicConnection && sessionManager.HasSession)
				{
					gameStateController.ChangeServer(null);
					return;
				}
				reconnectionAttempts = 0;
				showNetworkErrorPrompt("GlobalUI.ErrorMessages.NetworkError.Title", "GlobalUI.ErrorMessages.RetryFailureError");
				return;
			default:
				num = ((buttonFlag != DPrompt.ButtonFlags.OK) ? 1 : 0);
				break;
			case DPrompt.ButtonFlags.CANCEL:
				num = 0;
				break;
			}
			if (num == 0)
			{
				gameStateController.ExitWorld();
			}
		}

		private void onServerListLoaded(string path, GameObject serverList)
		{
			eventDispatcher.DispatchEvent(new PopupEvents.ShowPopup(UnityEngine.Object.Instantiate(serverList), true, true, "Accessibility.Popup.Title.ErrorServerList"));
		}

		private bool onRoomJoined(WorldServiceEvents.SelfRoomJoinedEvent evt)
		{
			if (reconnectionAttempts > 0)
			{
				reconnectionAttempts = 0;
			}
			return false;
		}
	}
}
