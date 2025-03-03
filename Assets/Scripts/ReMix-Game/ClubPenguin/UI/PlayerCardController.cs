using ClubPenguin.Analytics;
using ClubPenguin.CellPhone;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Igloo;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class PlayerCardController : MonoBehaviour
	{
		private const string OFFLINE_TEXT_TOKEN = "PlayerCard.OfflineText";

		private const string IN_ZONE_TOKEN = "PlayerCard.InZoneText";

		private const string IN_IGLOO_TOKEN = "PlayerCard.FriendStatus.OwnIgloo";

		private const string DIFFERENT_CLIENT_VERSION = "GlobalUI.JumpToFriend.Disabled";

		private const float MIN_JUMP_TO_FRIEND_DISTANCE = 3f;

		public Text NameText;

		public Text StatusText;

		public SpriteSelector MembershipSpriteSelector;

		public OnOffSpriteSelector OnlineSpriteSelector;

		public AvatarRenderTextureComponent AvatarRenderTextureComponent;

		public LevelProgressDisplay LevelProgressDisplay;

		public PlayerDetailsController DetailsController;

		public PlayerCardActionListController ActionListController;

		public PassportCodeController PassportCodeController;

		public GameObject LocalXpPanel;

		public GameObject LocalIglooButton;

		private CPDataEntityCollection dataEntityCollection;

		private EventChannel eventChannel;

		public DataEntityHandle Handle = DataEntityHandle.NullHandle;

		[HideInInspector]
		public FriendStatus FriendStatus;

		private CoinsData coinsData;

		private PresenceData presenceData;

		private ProfileData profileData;

		private MembershipData membershipData;

		[HideInInspector]
		public bool IsShowingJumpPrompt;

		public event Action<DataEntityHandle> OnHandleSet;

		public void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<FriendsServiceEvents.FriendsListUpdated>(onFriendsListUpdated);
			eventChannel.AddListener<FriendsServiceEvents.IncomingInvitationsListUpdated>(onIncomingInvitationsListUpdated);
			eventChannel.AddListener<FriendsServiceEvents.OutgoingInvitationsListUpdated>(onOutgoingInvitationsListUpdated);
			eventChannel.AddListener<PlayerCardEvents.DismissPlayerCard>(onPlayerCardDismiss);
			eventChannel.AddListener<PlayerCardEvents.SendFriendInvitation>(onSendFriendInvitation);
			eventChannel.AddListener<PlayerCardEvents.AcceptFriendInvitation>(onAcceptFriendInvitation);
			eventChannel.AddListener<PlayerCardEvents.ReportPlayer>(onReportPlayer);
			eventChannel.AddListener<PlayerCardEvents.JoinPlayer>(onJoinPlayer);
			eventChannel.AddListener<PlayerCardEvents.UnfriendPlayer>(onUnfriendPlayer);
			DetailsController.SetPlayerCardController(this);
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		public void SetUpPlayerCard(DataEntityHandle handle)
		{
			Handle = handle;
			if (this.OnHandleSet != null)
			{
				this.OnHandleSet.InvokeSafe(handle);
			}
			string displayName = dataEntityCollection.GetComponent<DisplayNameData>(handle).DisplayName;
			NameText.text = displayName;
			DetailsController.SetDisplayName(displayName);
			ActionListController.SetName(displayName);
			PassportCodeController.SetDisplayNameText(displayName);
			MembershipSpriteSelector.gameObject.SetActive(false);
			OnlineSpriteSelector.gameObject.SetActive(false);
			StatusText.gameObject.SetActive(false);
			FriendStatus = updateFriendStatus();
			PassportCodeController.gameObject.SetActive(false);
			LocalXpPanel.SetActive(true);
			if (FriendStatus == FriendStatus.Self)
			{
				if (dataEntityCollection.TryGetComponent(handle, out presenceData))
				{
					setUpPresenceData(presenceData);
				}
				else
				{
					Log.LogError(this, "Couldn't find presence data on the local player");
				}
				if (dataEntityCollection.TryGetComponent(handle, out profileData))
				{
					setUpProfileData(profileData);
				}
				else
				{
					Log.LogError(this, "Couldn't find profile data on the local player");
				}
				if (dataEntityCollection.TryGetComponent(handle, out membershipData))
				{
					setUpMembershipData(membershipData);
				}
				else
				{
					Log.LogError(this, "Couldn't find membership data on the local player");
				}
				coinsData = dataEntityCollection.GetComponent<CoinsData>(handle);
				setCoins(coinsData.Coins);
				coinsData.OnCoinsChanged += setCoins;
			}
			else
			{
				if (dataEntityCollection.TryGetComponent(handle, out presenceData))
				{
					presenceData.PresenceDataUpdated += onPresenceDataUpdated;
					setUpPresenceData(presenceData);
				}
				else
				{
					dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<PresenceData>>(onPresenceDataAdded);
				}
				if (dataEntityCollection.TryGetComponent(handle, out profileData))
				{
					profileData.ProfileDataUpdated += onProfileDataUpdated;
					setUpProfileData(profileData);
				}
				else
				{
					dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<ProfileData>>(onProfileDataAdded);
				}
				if (dataEntityCollection.TryGetComponent(handle, out membershipData))
				{
					membershipData.MembershipDataUpdated += onMembershipDataUpdated;
					setUpMembershipData(membershipData);
				}
				else
				{
					dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<MembershipData>>(onMembershipDataAdded);
				}
				SwidData component;
				if (dataEntityCollection.TryGetComponent(handle, out component))
				{
					Service.Get<INetworkServicesManager>().PlayerStateService.GetOtherPlayerDataBySwid(component.Swid);
				}
				else
				{
					Service.Get<INetworkServicesManager>().PlayerStateService.GetOtherPlayerDataByDisplayName(displayName);
				}
			}
			AvatarDetailsData component2;
			if (dataEntityCollection.TryGetComponent(handle, out component2))
			{
				AvatarRenderTextureComponent.RenderAvatar(component2);
			}
			else
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<AvatarDetailsData>>(onAvatarDetailsDataAdded);
			}
		}

		private bool onPresenceDataAdded(DataEntityEvents.ComponentAddedEvent<PresenceData> evt)
		{
			if (evt.Handle == Handle)
			{
				presenceData = evt.Component;
				presenceData.PresenceDataUpdated += onPresenceDataUpdated;
			}
			return false;
		}

		private void onPresenceDataUpdated(PresenceData presenceData)
		{
			setUpPresenceData(presenceData);
		}

		private bool onProfileDataAdded(DataEntityEvents.ComponentAddedEvent<ProfileData> evt)
		{
			if (evt.Handle == Handle)
			{
				profileData = evt.Component;
				profileData.ProfileDataUpdated += onProfileDataUpdated;
				setUpProfileData(profileData);
			}
			return false;
		}

		private void onProfileDataUpdated(ProfileData profileData)
		{
			setUpProfileData(profileData);
		}

		private bool onMembershipDataAdded(DataEntityEvents.ComponentAddedEvent<MembershipData> evt)
		{
			if (evt.Handle == Handle)
			{
				membershipData = evt.Component;
				membershipData.MembershipDataUpdated += onMembershipDataUpdated;
				setUpMembershipData(membershipData);
			}
			return false;
		}

		private void onMembershipDataUpdated(MembershipData membershipData)
		{
			setUpMembershipData(membershipData);
		}

		private bool onAvatarDetailsDataAdded(DataEntityEvents.ComponentAddedEvent<AvatarDetailsData> evt)
		{
			if (evt.Handle == Handle)
			{
				evt.Component.OnInitialized += onAvatarDetailsDataInitialized;
			}
			return false;
		}

		private void onAvatarDetailsDataInitialized(AvatarDetailsData avatarDetailsData)
		{
			avatarDetailsData.OnInitialized -= onAvatarDetailsDataInitialized;
			AvatarRenderTextureComponent.RenderAvatar(avatarDetailsData);
		}

		private void setUpPresenceData(PresenceData presenceData)
		{
			DetailsController.PresenceDataUpdated(FriendStatus, presenceData);
			Localizer localizer = Service.Get<Localizer>();
			bool flag = false;
			ZoneDefinition zoneDefinition = null;
			if (presenceData.IsInInstancedRoom && !string.IsNullOrEmpty(presenceData.InstanceRoom.name))
			{
				zoneDefinition = Service.Get<ZoneTransitionService>().GetZone(presenceData.InstanceRoom.name);
			}
			else if (!presenceData.IsInInstancedRoom && !string.IsNullOrEmpty(presenceData.Room))
			{
				zoneDefinition = Service.Get<ZoneTransitionService>().GetZone(presenceData.Room);
			}
			if (zoneDefinition != null)
			{
				OnlineSpriteSelector.IsOn = true;
				flag = true;
				bool flag2 = false;
				PresenceData component;
				if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
				{
					flag2 = (component.ContentIdentifier == presenceData.ContentIdentifier);
				}
				if (flag2)
				{
					if (zoneDefinition.Type == ZoneDefinition.ZoneType.Igloo)
					{
						StatusText.text = localizer.GetTokenTranslation("PlayerCard.FriendStatus.OwnIgloo");
					}
					else
					{
						StatusText.text = localizer.GetTokenTranslationFormatted("PlayerCard.InZoneText", zoneDefinition.ZoneToken);
					}
				}
				else
				{
					StatusText.text = localizer.GetTokenTranslation("GlobalUI.JumpToFriend.Disabled");
				}
			}
			if (!flag)
			{
				StatusText.text = localizer.GetTokenTranslation("PlayerCard.OfflineText");
				OnlineSpriteSelector.IsOn = false;
			}
			StatusText.gameObject.SetActive(true);
		}

		private void setUpProfileData(ProfileData profileData)
		{
			bool flag = FriendStatus == FriendStatus.Self;
			DetailsController.SetPenguinAge(profileData.PenguinAgeInDays);
			PassportCodeController.SetAgeText(profileData.PenguinAgeInDays);
			OnlineSpriteSelector.IsOn = profileData.IsOnline;
			PassportCodeController.SetUpLevelText(flag, profileData);
			MembershipSpriteSelector.gameObject.SetActive(true);
			OnlineSpriteSelector.gameObject.SetActive(true);
			LevelProgressDisplay.SetUpProgression(flag, profileData);
			LocalIglooButton.SetActive(flag);
		}

		private void setUpMembershipData(MembershipData membershipData)
		{
			int index = 0;
			switch (membershipData.MembershipType)
			{
			case MembershipType.Member:
				index = 1;
				break;
			case MembershipType.AllAccessEventMember:
				index = 2;
				break;
			}
			MembershipSpriteSelector.SelectSprite(index);
		}

		private bool onPlayerCardDismiss(PlayerCardEvents.DismissPlayerCard evt)
		{
			destroy();
			return true;
		}

		private bool onSendFriendInvitation(PlayerCardEvents.SendFriendInvitation evt)
		{
			if (FriendsDataModelService.FriendsList.Count < FriendsDataModelService.MaxFriendsCount)
			{
				DetailsController.SetPreloaderActive(true);
				string displayName = dataEntityCollection.GetComponent<DisplayNameData>(Handle).DisplayName;
				Service.Get<INetworkServicesManager>().FriendsService.FindUser(displayName, Service.Get<SessionManager>().LocalUser);
				eventChannel.AddListener<FriendsServiceEvents.FindUserSent>(onFindUserSent);
			}
			else
			{
				Service.Get<PromptManager>().ShowPrompt("MaximumFriendsPrompt", null);
			}
			return true;
		}

		private bool onFindUserSent(FriendsServiceEvents.FindUserSent evt)
		{
			eventChannel.RemoveListener<FriendsServiceEvents.FindUserSent>(onFindUserSent);
			if (evt.Success)
			{
				onFindUserSuccess();
			}
			else
			{
				onFindUserFailed();
			}
			return false;
		}

		private void onFindUserSuccess()
		{
			eventChannel.AddListener<FriendsServiceEvents.SendFriendInvitationSent>(onFriendInvitationSent);
			SearchedUserData component = dataEntityCollection.GetComponent<SearchedUserData>(Handle);
			Service.Get<INetworkServicesManager>().FriendsService.SendFriendInvitation(component.SearchedUser, Service.Get<SessionManager>().LocalUser);
		}

		private void onFindUserFailed()
		{
			onFriendInvitationFailed();
		}

		private bool onFriendInvitationSent(FriendsServiceEvents.SendFriendInvitationSent evt)
		{
			eventChannel.RemoveListener<FriendsServiceEvents.SendFriendInvitationSent>(onFriendInvitationSent);
			if (evt.Success)
			{
				DetailsController.SetFriendStatus(FriendStatus.OutgoingInvite);
			}
			else
			{
				onFriendInvitationFailed();
			}
			return false;
		}

		private void onFriendInvitationFailed()
		{
			DetailsController.SetFriendStatus(FriendStatus.None);
		}

		private bool onAcceptFriendInvitation(PlayerCardEvents.AcceptFriendInvitation evt)
		{
			if (FriendsDataModelService.FriendsList.Count < FriendsDataModelService.MaxFriendsCount)
			{
				DetailsController.SetPreloaderActive(true);
				IncomingFriendInvitationData component = dataEntityCollection.GetComponent<IncomingFriendInvitationData>(Handle);
				Service.Get<INetworkServicesManager>().FriendsService.AcceptFriendInvitation(component.Invitation, Service.Get<SessionManager>().LocalUser);
			}
			else
			{
				Service.Get<PromptManager>().ShowPrompt("MaximumFriendsPrompt", null);
			}
			return false;
		}

		private bool onReportPlayer(PlayerCardEvents.ReportPlayer evt)
		{
			OpenReportPlayerCommand openReportPlayerCommand = new OpenReportPlayerCommand(Handle);
			openReportPlayerCommand.Execute();
			return false;
		}

		private bool onJoinPlayer(PlayerCardEvents.JoinPlayer evt)
		{
			PresenceData component = dataEntityCollection.GetComponent<PresenceData>(Handle);
			PresenceData component2 = dataEntityCollection.GetComponent<PresenceData>(dataEntityCollection.LocalPlayerHandle);
			if (component == null || component2 == null)
			{
				return false;
			}
			if (isLocalAndRemoteInTheSameRoom(component, component2))
			{
				eventChannel.AddListener<FriendsServiceEvents.FriendLocationInRoomReceived>(onFriendLocationReceived);
				eventChannel.AddListener<FriendsServiceEvents.FriendNotInRoom>(onFriendNotInRoom);
				Service.Get<INetworkServicesManager>().FriendsService.GetFriendLocationInRoom(dataEntityCollection.GetComponent<SwidData>(Handle).Swid);
			}
			else
			{
				SpawnAtPlayerData spawnAtPlayerData = dataEntityCollection.AddComponent<SpawnAtPlayerData>(dataEntityCollection.LocalPlayerHandle);
				spawnAtPlayerData.PlayerSWID = dataEntityCollection.GetComponent<SwidData>(Handle).Swid;
				string onJoinNotificationTag = string.Empty;
				if (component.World != component2.World)
				{
					onJoinNotificationTag = "GlobalUI.Notification.WorldSwitched";
				}
				if (component.IsInInstancedRoom)
				{
					Language language = Service.Get<Localizer>().Language;
					WorldDefinition world = Service.Get<ZoneTransitionService>().GetWorld(component.World);
					if (world != null)
					{
						language = world.Language;
					}
					Service.Get<ZoneTransitionService>().LoadIgloo(component.InstanceRoom, language, SceneStateData.SceneState.Play, "Loading", onJoinNotificationTag);
				}
				else
				{
					Service.Get<ZoneTransitionService>().LoadZone(component.Room, "Loading", component.World, onJoinNotificationTag);
				}
			}
			Service.Get<ICPSwrveService>().Action("game.friends", "jump");
			return false;
		}

		private bool isLocalAndRemoteInTheSameRoom(PresenceData remotePresenceData, PresenceData localPresenceData)
		{
			if (localPresenceData.IsNotInCurrentRoomsScene)
			{
				return false;
			}
			return (!localPresenceData.IsInInstancedRoom && remotePresenceData.Room == localPresenceData.Room && remotePresenceData.World == localPresenceData.World) || (localPresenceData.IsInInstancedRoom && dataEntityCollection.HasComponent<GameObjectReferenceData>(Handle));
		}

		private bool onFriendNotInRoom(FriendsServiceEvents.FriendNotInRoom evt)
		{
			eventChannel.RemoveListener<FriendsServiceEvents.FriendLocationInRoomReceived>(onFriendLocationReceived);
			eventChannel.RemoveListener<FriendsServiceEvents.FriendNotInRoom>(onFriendNotInRoom);
			destroyAndCloseCellPhone();
			return false;
		}

		private bool onFriendLocationReceived(FriendsServiceEvents.FriendLocationInRoomReceived evt)
		{
			eventChannel.RemoveListener<FriendsServiceEvents.FriendLocationInRoomReceived>(onFriendLocationReceived);
			eventChannel.RemoveListener<FriendsServiceEvents.FriendNotInRoom>(onFriendNotInRoom);
			Vector3 location = evt.Location;
			if (!(Vector3.Distance(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.transform.position, location) <= 3f))
			{
				LocomotionTracker component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<LocomotionTracker>();
				GameObjectReferenceData component2;
				if (component.IsCurrentControllerOfType<SlideController>())
				{
					component.SetCurrentController<RunController>();
				}
				else if (component.IsCurrentControllerOfType<SwimController>() && dataEntityCollection.TryGetComponent(Handle, out component2))
				{
					LocomotionTracker component3 = component2.GameObject.GetComponent<LocomotionTracker>();
					if (component3.IsCurrentControllerOfType<RunController>())
					{
						Animator component4 = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<Animator>();
						component4.SetTrigger(AnimationHashes.Params.SwimToWalk);
						component.SetCurrentController<RunController>();
					}
				}
				if (component.IsCurrentControllerOfType<RunController>())
				{
					location.y += 0.5f;
				}
				SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.transform.position = location;
				ClubPenguin.Core.SceneRefs.Get<BaseCamera>().Snap();
				CoroutineRunner.Start(LocomotionUtils.nudgePlayer(component), component.gameObject, "MoveAfterJump");
			}
			destroyAndCloseCellPhone();
			return false;
		}

		private void destroyAndCloseCellPhone()
		{
			destroy();
			Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.CellPhoneClosed));
			StateMachineContext component = SceneRefs.UiTrayRoot.GetComponent<StateMachineContext>();
			if (component != null)
			{
				component.SendEvent(new ExternalEvent("Root", "mainnav_locomotion"));
			}
		}

		private bool onUnfriendPlayer(PlayerCardEvents.UnfriendPlayer evt)
		{
			DetailsController.SetPreloaderActive(true);
			FriendData component = dataEntityCollection.GetComponent<FriendData>(Handle);
			Service.Get<INetworkServicesManager>().FriendsService.Unfriend(component.Friend, Service.Get<SessionManager>().LocalUser);
			return false;
		}

		private void setCoins(int coins)
		{
			DetailsController.SetCoins(coins);
		}

		private bool onFriendsListUpdated(FriendsServiceEvents.FriendsListUpdated evt)
		{
			updateFriendStatus();
			return false;
		}

		private bool onIncomingInvitationsListUpdated(FriendsServiceEvents.IncomingInvitationsListUpdated evt)
		{
			updateFriendStatus();
			return false;
		}

		private bool onOutgoingInvitationsListUpdated(FriendsServiceEvents.OutgoingInvitationsListUpdated evt)
		{
			updateFriendStatus();
			return false;
		}

		private FriendStatus updateFriendStatus()
		{
			FriendStatus = FriendsDataModelService.GetFriendStatus(Handle);
			if (DetailsController != null)
			{
				DetailsController.SetFriendStatus(FriendStatus);
			}
			if (ActionListController != null)
			{
				ActionListController.SetFriendStatus(FriendStatus);
			}
			return FriendStatus;
		}

		public void OnCloseButtonClicked()
		{
			destroy();
		}

		private void destroy()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerCardEvents.PlayerCardClosed));
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void OnDestroy()
		{
			DataEntityHandle entityByType = dataEntityCollection.GetEntityByType<PlayerCardData>();
			PlayerCardData component = dataEntityCollection.GetComponent<PlayerCardData>(entityByType);
			component.IsPlayerCardShowing = false;
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (dataEntityCollection != null)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<PresenceData>>(onPresenceDataAdded);
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<ProfileData>>(onProfileDataAdded);
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<MembershipData>>(onMembershipDataAdded);
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<AvatarDetailsData>>(onAvatarDetailsDataAdded);
			}
			if (coinsData != null)
			{
				coinsData.OnCoinsChanged -= setCoins;
			}
			if (presenceData != null)
			{
				presenceData.PresenceDataUpdated -= onPresenceDataUpdated;
			}
			if (profileData != null)
			{
				profileData.ProfileDataUpdated -= onProfileDataUpdated;
			}
			if (membershipData != null)
			{
				membershipData.MembershipDataUpdated -= onMembershipDataUpdated;
			}
			this.OnHandleSet = null;
		}
	}
}
