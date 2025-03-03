using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Mix.Native;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public abstract class AbstractPlayerListController : MonoBehaviour
	{
		protected const string IDLE_CACHE_CONTEXT = "FriendsListIdle";

		protected const string SLEEPING_CACHE_CONTEXT = "FriendsListSleeping";

		protected const float IDLE_TIME = 0.5f;

		protected const float SLEEPING_TIME = 0.75f;

		public PrefabContentKey PlayerItemContentKey;

		public string IdlePenguinState;

		public string SleepingPenguinState;

		public bool ShowPlayerOfflineState = true;

		protected List<DataEntityHandle> allPlayersList = new List<DataEntityHandle>();

		protected List<DataEntityHandle> visiblePlayersList = new List<DataEntityHandle>();

		protected VerticalGridPooledScrollRect pooledScrollRect;

		protected AvatarImageComponent playerAvatarRenderer;

		protected EventChannel eventChannel;

		protected CPDataEntityCollection dataEntityCollection;

		protected List<ProfileData> profileDataList;

		protected List<MembershipData> membershipDataList;

		protected bool isInitialized;

		protected bool onlineStatusReceived;

		protected GameObject playerItemPrefab;

		private bool initializePoolWhenPlayerItemPrefabIsLoaded;

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			awake();
		}

		protected virtual void awake()
		{
		}

		private void Start()
		{
			initializePoolWhenPlayerItemPrefabIsLoaded = false;
			profileDataList = new List<ProfileData>();
			membershipDataList = new List<MembershipData>();
			pooledScrollRect = GetComponentInChildren<VerticalGridPooledScrollRect>();
			addPooledScrollRectObservers();
			playerAvatarRenderer = GetComponent<AvatarImageComponent>();
			AvatarImageComponent avatarImageComponent = playerAvatarRenderer;
			avatarImageComponent.OnImageReady = (Action<DataEntityHandle, Texture2D>)Delegate.Combine(avatarImageComponent.OnImageReady, new Action<DataEntityHandle, Texture2D>(onImageReady));
			initPlayersList();
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<ProfileData>>(onProfileDataAdded);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<MembershipData>>(onMembershipDataAdded);
			PromptManager promptManager = Service.Get<PromptManager>();
			promptManager.PromptCreated = (Action<GameObject>)Delegate.Combine(promptManager.PromptCreated, new Action<GameObject>(onPromptCreated));
			List<string> playerSwids = getPlayerSwids();
			if (playerSwids.Count > 0)
			{
				Service.Get<INetworkServicesManager>().PlayerStateService.GetOnlinePlayersBySwids(playerSwids);
			}
			else
			{
				onlineStatusReceived = true;
			}
			Content.LoadAsync(onPlayerItemPrefabLoaded, PlayerItemContentKey);
			start();
			isInitialized = true;
		}

		protected void addPooledScrollRectObservers()
		{
			pooledScrollRect.ObjectAdded += onObjectAdded;
			pooledScrollRect.ObjectRemoved += onObjectRemoved;
			pooledScrollRect.OnIsMovingChanged += onIsMovingChanged;
			pooledScrollRect.OnRefreshCompleted += onRefreshCompleted;
		}

		protected virtual void start()
		{
		}

		private void OnDestroy()
		{
			if (playerAvatarRenderer != null)
			{
				AvatarImageComponent avatarImageComponent = playerAvatarRenderer;
				avatarImageComponent.OnImageReady = (Action<DataEntityHandle, Texture2D>)Delegate.Remove(avatarImageComponent.OnImageReady, new Action<DataEntityHandle, Texture2D>(onImageReady));
			}
			if (dataEntityCollection != null)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<ProfileData>>(onProfileDataAdded);
			}
			if (profileDataList != null)
			{
				for (int i = 0; i < profileDataList.Count; i++)
				{
					profileDataList[i].ProfileDataUpdated -= onProfileDataUpdated;
				}
			}
			if (membershipDataList != null)
			{
				for (int i = 0; i < membershipDataList.Count; i++)
				{
					membershipDataList[i].MembershipDataUpdated -= onMembershipDataUpdated;
				}
			}
			removePooledScrollRectObservers();
			PromptManager promptManager = Service.Get<PromptManager>();
			promptManager.PromptCreated = (Action<GameObject>)Delegate.Remove(promptManager.PromptCreated, new Action<GameObject>(onPromptCreated));
			onDestroy();
		}

		protected void removePooledScrollRectObservers()
		{
			if (pooledScrollRect != null)
			{
				pooledScrollRect.ObjectAdded -= onObjectAdded;
				pooledScrollRect.ObjectRemoved -= onObjectRemoved;
				pooledScrollRect.OnIsMovingChanged -= onIsMovingChanged;
				pooledScrollRect.OnRefreshCompleted -= onRefreshCompleted;
			}
		}

		protected virtual void onDestroy()
		{
		}

		private void OnEnable()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PlayerStateServiceEvents.OnlinePlayerSwidListReceived>(onOnlinePlayerSwidListReceived);
			eventChannel.AddListener<PopupEvents.ShowTopPopup>(onTopPopupShown);
			removePooledScrollRectObservers();
			onEnable();
		}

		protected virtual void onEnable()
		{
		}

		private void OnDisable()
		{
			onDisable();
		}

		protected virtual void onDisable()
		{
		}

		public void Show()
		{
			pooledScrollRect.gameObject.SetActive(true);
		}

		public void Hide()
		{
			pooledScrollRect.gameObject.SetActive(false);
		}

		protected virtual void initPlayersList()
		{
		}

		private List<string> getPlayerSwids()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < allPlayersList.Count; i++)
			{
				SwidData component;
				if (dataEntityCollection.TryGetComponent(allPlayersList[i], out component) && !string.IsNullOrEmpty(component.Swid))
				{
					list.Add(component.Swid);
				}
			}
			return list;
		}

		private bool onOnlinePlayerSwidListReceived(PlayerStateServiceEvents.OnlinePlayerSwidListReceived evt)
		{
			onlineStatusReceived = true;
			ProfileData component;
			for (int i = 0; i < allPlayersList.Count; i++)
			{
				if (!dataEntityCollection.TryGetComponent(allPlayersList[i], out component))
				{
					component = dataEntityCollection.AddComponent<ProfileData>(allPlayersList[i]);
				}
				component.IsOnline = false;
			}
			for (int i = 0; i < evt.Swids.Count; i++)
			{
				DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SwidData, string>(evt.Swids[i]);
				if (!dataEntityHandle.IsNull)
				{
					if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
					{
						component = dataEntityCollection.AddComponent<ProfileData>(dataEntityHandle);
					}
					component.IsOnline = true;
				}
			}
			sortPlayers(visiblePlayersList);
			if (playerItemPrefab != null)
			{
				initializePool();
			}
			return false;
		}

		protected virtual bool onProfileDataAdded(DataEntityEvents.ComponentAddedEvent<ProfileData> evt)
		{
			evt.Component.ProfileDataUpdated += onProfileDataUpdated;
			profileDataList.Add(evt.Component);
			return false;
		}

		protected virtual void onProfileDataUpdated(ProfileData profileData)
		{
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(profileData);
			int poolIndexFromHandle = getPoolIndexFromHandle(entityByComponent);
			if (poolIndexFromHandle >= 0 && pooledScrollRect.IsIndexCellVisible(poolIndexFromHandle))
			{
				string displayName = dataEntityCollection.GetComponent<DisplayNameData>(entityByComponent).DisplayName;
				AbstractPlayerListItem component = pooledScrollRect.GetCellAtIndex(poolIndexFromHandle).GetComponent<AbstractPlayerListItem>();
				bool flag = getIsOnline(entityByComponent) || !ShowPlayerOfflineState;
				component.SetOnlineStatus(flag);
				component.UpdateProfileData(profileData);
				renderPlayer(component, entityByComponent, displayName, flag);
			}
		}

		protected virtual bool onMembershipDataAdded(DataEntityEvents.ComponentAddedEvent<MembershipData> evt)
		{
			evt.Component.MembershipDataUpdated += onMembershipDataUpdated;
			membershipDataList.Add(evt.Component);
			return false;
		}

		protected virtual void onMembershipDataUpdated(MembershipData membershipData)
		{
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(membershipData);
			int poolIndexFromHandle = getPoolIndexFromHandle(entityByComponent);
			if (poolIndexFromHandle >= 0 && pooledScrollRect.IsIndexCellVisible(poolIndexFromHandle))
			{
				AbstractPlayerListItem component = pooledScrollRect.GetCellAtIndex(poolIndexFromHandle).GetComponent<AbstractPlayerListItem>();
				component.SetMembershipType(getMembershipType(entityByComponent));
			}
		}

		private void onPlayerItemPrefabLoaded(string path, GameObject prefab)
		{
			playerItemPrefab = prefab;
			if (onlineStatusReceived || initializePoolWhenPlayerItemPrefabIsLoaded)
			{
				initializePool();
			}
		}

		protected virtual void initializePool()
		{
			if (!pooledScrollRect.IsInitialized)
			{
				if (playerItemPrefab == null)
				{
					initializePoolWhenPlayerItemPrefabIsLoaded = true;
				}
				else
				{
					pooledScrollRect.Init(getPoolCount(), playerItemPrefab);
				}
			}
			else
			{
				pooledScrollRect.RefreshList(visiblePlayersList.Count);
			}
		}

		protected virtual void onObjectAdded(RectTransform item, int poolIndex)
		{
			setUpObject(item, poolIndex);
		}

		protected virtual void setUpObject(RectTransform item, int poolIndex)
		{
			AbstractPlayerListItem component = item.GetComponent<AbstractPlayerListItem>();
			DataEntityHandle handleFromPoolIndex = getHandleFromPoolIndex(poolIndex);
			AbstractPlayerListItem component2 = item.GetComponent<AbstractPlayerListItem>();
			string displayName2 = component2.Name = dataEntityCollection.GetComponent<DisplayNameData>(handleFromPoolIndex).DisplayName;
			component2.SetPlayer(handleFromPoolIndex);
			ProfileData component3;
			bool flag = dataEntityCollection.TryGetComponent(handleFromPoolIndex, out component3);
			if (!flag)
			{
				Service.Get<OtherPlayerDetailsRequestBatcher>().RequestOtherPlayerDetails(handleFromPoolIndex);
			}
			if (flag)
			{
				bool flag2 = getIsOnline(handleFromPoolIndex) || !ShowPlayerOfflineState;
				component.SetOnlineStatus(flag2);
				renderPlayer(component, handleFromPoolIndex, displayName2, flag2);
			}
		}

		protected void renderPlayer(AbstractPlayerListItem playerListItem, DataEntityHandle handle, string displayName, bool isOnline)
		{
			if (!playerListItem.IsRendered)
			{
				playerListItem.IsRendered = true;
				playerListItem.SetPreloaderActive(true);
				playerListItem.SetAvatarIconActive(false);
				if (playerAvatarRenderer.IsRenderInProgress(displayName))
				{
					playerAvatarRenderer.CancelRender(displayName);
				}
				if (isOnline)
				{
					AvatarAnimationFrame avatarAnimationFrame = new AvatarAnimationFrame(IdlePenguinState, 0.5f);
					playerAvatarRenderer.RequestImage(handle, avatarAnimationFrame, "FriendsListIdle");
				}
				else
				{
					AvatarAnimationFrame avatarAnimationFrame = new AvatarAnimationFrame(SleepingPenguinState, 0.75f);
					playerAvatarRenderer.RequestImage(handle, avatarAnimationFrame, "FriendsListSleeping");
				}
			}
		}

		protected virtual void onObjectRemoved(RectTransform item, int poolIndex)
		{
			AbstractPlayerListItem component = item.GetComponent<AbstractPlayerListItem>();
			component.Reset();
			DataEntityHandle handleFromPoolIndex = getHandleFromPoolIndex(poolIndex);
			if (!handleFromPoolIndex.IsNull)
			{
				string displayName = dataEntityCollection.GetComponent<DisplayNameData>(handleFromPoolIndex).DisplayName;
				if (playerAvatarRenderer.IsRenderInProgress(displayName))
				{
					playerAvatarRenderer.CancelRender(displayName);
				}
			}
		}

		private void onIsMovingChanged(bool isMoving)
		{
			playerAvatarRenderer.IsRenderingActive = !isMoving;
		}

		protected virtual void onRefreshCompleted()
		{
		}

		private void onImageReady(DataEntityHandle handle, Texture2D icon)
		{
			int poolIndexFromHandle = getPoolIndexFromHandle(handle);
			if (poolIndexFromHandle >= 0 && pooledScrollRect.IsIndexCellVisible(poolIndexFromHandle))
			{
				AbstractPlayerListItem component = pooledScrollRect.GetCellAtIndex(poolIndexFromHandle).GetComponent<AbstractPlayerListItem>();
				component.SetPreloaderActive(false);
				component.SetAvatarIcon(icon);
				component.SetAvatarIconActive(true);
			}
		}

		private void onPromptCreated(GameObject prompt)
		{
			Service.Get<NativeKeyboardManager>().Keyboard.Hide();
		}

		private bool onTopPopupShown(PopupEvents.ShowTopPopup evt)
		{
			Service.Get<NativeKeyboardManager>().Keyboard.Hide();
			return false;
		}

		protected virtual DataEntityHandle getHandleFromPoolIndex(int poolIndex)
		{
			return visiblePlayersList[poolIndex];
		}

		protected virtual int getPoolIndexFromHandle(DataEntityHandle handle)
		{
			return getIndexFromList(visiblePlayersList, handle);
		}

		protected int getIndexFromList(List<DataEntityHandle> handles, DataEntityHandle handle)
		{
			return handles.FindIndex((DataEntityHandle x) => !x.IsNull && !handle.IsNull && dataEntityCollection.GetComponent<DisplayNameData>(x).DisplayName == dataEntityCollection.GetComponent<DisplayNameData>(handle).DisplayName);
		}

		protected virtual int getPoolCount()
		{
			return visiblePlayersList.Count;
		}

		protected abstract void sortPlayers(List<DataEntityHandle> listToSort);

		private bool getIsOnline(DataEntityHandle handle)
		{
			ProfileData component = dataEntityCollection.GetComponent<ProfileData>(handle);
			return component != null && component.IsOnline;
		}

		protected MembershipType getMembershipType(DataEntityHandle handle)
		{
			MembershipData component = dataEntityCollection.GetComponent<MembershipData>(handle);
			return (component != null) ? component.MembershipType : MembershipType.None;
		}
	}
}
