using ClubPenguin.Chat;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(AvatarImageComponent))]
	[RequireComponent(typeof(VerticalScrollingLayoutElementPool))]
	public class FullScreenChatController : MonoBehaviour
	{
		private struct FullScreenChatBlockData
		{
			public long SessionId;

			public string DisplayName;

			public string Message;

			public AvatarDetailsData AvatarDetailsData;

			public bool IsChatActivity;

			public bool IsAwatingModeration;

			public bool IsChatBlocked;

			public FullScreenChatBlockData(long sessionId, string displayName, string message, AvatarDetailsData avatarDetailsData, bool isChatActivity = false, bool isAwatingModeration = false, bool isChatBlocked = false)
			{
				SessionId = sessionId;
				DisplayName = displayName;
				Message = message;
				AvatarDetailsData = avatarDetailsData;
				IsChatActivity = isChatActivity;
				IsAwatingModeration = isAwatingModeration;
				IsChatBlocked = isChatBlocked;
			}
		}

		private const int localChatBlockIndex = 0;

		private const int remoteChatBlockIndex = 1;

		private const int friendChatBlockIndex = 2;

		private const string FULL_SCREEN_CHAT_CONTEXT = "FullScreenChatAvatar";

		private const string IDLE_PENGUIN = "Base Layer.Idle";

		private const float IDLE_TIME = 0.5f;

		public Transform ChatPanel;

		public RuntimeAnimatorController PenguinAnimatorController;

		public Button JumpToNewMessageButton;

		private CPDataEntityCollection dataEntityCollection;

		private VerticalScrollingLayoutElementPool layoutElementPool;

		private AvatarImageComponent avatarImageComponent;

		private Dictionary<long, DataEntityHandle> handles;

		private List<FullScreenChatBlockData> fullScreenChatBlockDataList;

		private Queue<long> imageRequests;

		private bool isPoolReady;

		private GameObject localChatBlockPrefab;

		private GameObject remoteChatBlockPrefab;

		private GameObject friendChatBlockPrefab;

		[SerializeField]
		private PrefabContentKey localChatBlockContentKey;

		[SerializeField]
		private PrefabContentKey remoteChatBlockContentKey;

		[SerializeField]
		private PrefabContentKey friendChatBlockContentKey;

		private EventChannel eventChannel;

		private void Start()
		{
			ClubPenguin.Core.SceneRefs.Get<CameraRenderingControl>().DisableRendering(true, false);
			JumpToNewMessageButton.gameObject.SetActive(false);
			JumpToNewMessageButton.onClick.AddListener(onJumpToNewMessageButtonClicked);
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			layoutElementPool = GetComponent<VerticalScrollingLayoutElementPool>();
			VerticalScrollingLayoutElementPool verticalScrollingLayoutElementPool = layoutElementPool;
			verticalScrollingLayoutElementPool.OnPoolReady = (System.Action)Delegate.Combine(verticalScrollingLayoutElementPool.OnPoolReady, new System.Action(onPoolReady));
			VerticalScrollingLayoutElementPool verticalScrollingLayoutElementPool2 = layoutElementPool;
			verticalScrollingLayoutElementPool2.OnElementShown = (Action<int, GameObject>)Delegate.Combine(verticalScrollingLayoutElementPool2.OnElementShown, new Action<int, GameObject>(onElementShown));
			VerticalScrollingLayoutElementPool verticalScrollingLayoutElementPool3 = layoutElementPool;
			verticalScrollingLayoutElementPool3.OnElementHidden = (Action<int, GameObject>)Delegate.Combine(verticalScrollingLayoutElementPool3.OnElementHidden, new Action<int, GameObject>(onElementHidden));
			avatarImageComponent = GetComponent<AvatarImageComponent>();
			AvatarImageComponent obj = avatarImageComponent;
			obj.OnImageReady = (Action<DataEntityHandle, Texture2D>)Delegate.Combine(obj.OnImageReady, new Action<DataEntityHandle, Texture2D>(onImageReady));
			imageRequests = new Queue<long>();
			ChatHistoryData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				parseChatHistory(component.MessageHistory);
			}
			Content.LoadAsync(onLocalChatBlockLoaded, localChatBlockContentKey);
			Content.LoadAsync(onRemoteChatBlockLoaded, remoteChatBlockContentKey);
			Content.LoadAsync(onFriendChatBlockLoaded, friendChatBlockContentKey);
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<ChatServiceEvents.ChatMessageReceived>(chatMessageReceived);
			eventChannel.AddListener<ChatServiceEvents.ChatMessageBlockedReceived>(onChatMessageBlockedReceived);
			eventChannel.AddListener<ChatMessageSender.SendChatMessage>(onSendChatMessage);
			eventChannel.AddListener<ChatServiceEvents.SendChatActivity>(onSendChatActivity);
			eventChannel.AddListener<ChatServiceEvents.SendChatActivityCancel>(onSendChatActivityCancel);
			eventChannel.AddListener<ChatServiceEvents.ChatActivityReceived>(onChatActivityReceived);
			eventChannel.AddListener<ChatServiceEvents.ChatActivityCancelReceived>(onChatActivityCancelReceived);
		}

		private void Update()
		{
			if (JumpToNewMessageButton.gameObject.activeSelf && layoutElementPool.ElementScrollRect.verticalNormalizedPosition <= 0f)
			{
				JumpToNewMessageButton.gameObject.SetActive(false);
			}
		}

		private void onJumpToNewMessageButtonClicked()
		{
			jumpToNewMessage();
		}

		private void jumpToNewMessage()
		{
			layoutElementPool.ElementScrollRect.verticalNormalizedPosition = 0f;
			JumpToNewMessageButton.gameObject.SetActive(false);
		}

		private void parseChatHistory(IList<DChatMessage> chatHistory)
		{
			handles = new Dictionary<long, DataEntityHandle>();
			fullScreenChatBlockDataList = new List<FullScreenChatBlockData>();
			for (int i = 0; i < chatHistory.Count; i++)
			{
				DataEntityHandle handle = findPlayerHandle(chatHistory[i].PlayerId);
				addChatMessageToData(chatHistory[i].PlayerId, chatHistory[i].Message, handle);
			}
		}

		private DataEntityHandle findPlayerHandle(long sessionId)
		{
			DataEntityHandle value;
			if (!handles.TryGetValue(sessionId, out value))
			{
				value = dataEntityCollection.FindEntity<SessionIdData, long>(sessionId);
				handles.Add(sessionId, value);
			}
			return value;
		}

		private void addChatMessageToData(long sessionId, string message, DataEntityHandle handle, bool isChatActivity = false, bool isAwaitingModeration = false, bool isChatBlocked = false)
		{
			AvatarDetailsData component;
			DisplayNameData component2;
			FullScreenChatBlockData item;
			if (!handle.IsNull && dataEntityCollection.TryGetComponent(handle, out component) && dataEntityCollection.TryGetComponent(handle, out component2))
			{
				item = new FullScreenChatBlockData(sessionId, component2.DisplayName, message, component, isChatActivity, isAwaitingModeration, isChatBlocked);
			}
			else
			{
				item = new FullScreenChatBlockData(sessionId, string.Empty, message, null, isChatActivity, isAwaitingModeration, isChatBlocked);
				Log.LogError(this, "Could not get a display name and avatar details for this player");
			}
			fullScreenChatBlockDataList.Add(item);
		}

		private void onPoolReady()
		{
			VerticalScrollingLayoutElementPool verticalScrollingLayoutElementPool = layoutElementPool;
			verticalScrollingLayoutElementPool.OnPoolReady = (System.Action)Delegate.Remove(verticalScrollingLayoutElementPool.OnPoolReady, new System.Action(onPoolReady));
			isPoolReady = true;
			initializePool();
		}

		private void onLocalChatBlockLoaded(string path, GameObject localChatBlockPrefab)
		{
			this.localChatBlockPrefab = localChatBlockPrefab;
			checkChatBlocksLoaded();
		}

		private void onRemoteChatBlockLoaded(string path, GameObject remoteChatBlockPrefab)
		{
			this.remoteChatBlockPrefab = remoteChatBlockPrefab;
			checkChatBlocksLoaded();
		}

		private void onFriendChatBlockLoaded(string path, GameObject friendChatBlockPrefab)
		{
			this.friendChatBlockPrefab = friendChatBlockPrefab;
			checkChatBlocksLoaded();
		}

		private void checkChatBlocksLoaded()
		{
			if (localChatBlockPrefab != null && remoteChatBlockPrefab != null && friendChatBlockPrefab != null)
			{
				GameObject[] prefabsToInstance = new GameObject[3]
				{
					localChatBlockPrefab,
					remoteChatBlockPrefab,
					friendChatBlockPrefab
				};
				layoutElementPool.SetPrefabsToInstance(prefabsToInstance);
			}
		}

		private void initializePool()
		{
			for (int i = 0; i < fullScreenChatBlockDataList.Count; i++)
			{
				addChatMessageToPool(fullScreenChatBlockDataList[i].SessionId);
			}
		}

		private void addChatMessageToPool(long sessionId)
		{
			float y = layoutElementPool.ElementScrollRect.content.anchoredPosition.y;
			float y2 = layoutElementPool.ElementScrollRect.content.sizeDelta.y;
			if (dataEntityCollection.IsLocalPlayer(sessionId))
			{
				layoutElementPool.AddElement(1);
			}
			else
			{
				DataEntityHandle b = findPlayerHandle(sessionId);
				List<DataEntityHandle> friendsList = FriendsDataModelService.FriendsList;
				bool flag = false;
				for (int i = 0; i < friendsList.Count; i++)
				{
					if (friendsList[i] == b)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					layoutElementPool.AddElement(1, 2);
				}
				else
				{
					layoutElementPool.AddElement(1, 1);
				}
			}
			if (y < -20f)
			{
				StartCoroutine(scrollToOriginalPosition(y2, y));
			}
		}

		private IEnumerator scrollToOriginalPosition(float oldHeight, float oldPosition)
		{
			int frame = 0;
			for (int i = 0; i < 2; i++)
			{
				float originalScrollHeight = layoutElementPool.ElementScrollRect.content.sizeDelta.y;
				while (layoutElementPool.ElementScrollRect.content.sizeDelta.y == originalScrollHeight)
				{
					frame++;
					if (frame > 30)
					{
						yield break;
					}
					yield return new WaitForEndOfFrame();
				}
			}
			float difference = layoutElementPool.ElementScrollRect.content.sizeDelta.y - oldHeight;
			layoutElementPool.ElementScrollRect.content.anchoredPosition = new Vector2(layoutElementPool.ElementScrollRect.content.anchoredPosition.x, oldPosition - difference);
		}

		private void onElementShown(int index, GameObject element)
		{
			FullScreenChatBlock component = element.GetComponent<FullScreenChatBlock>();
			component.SessionId = fullScreenChatBlockDataList[index].SessionId;
			component.SetChatMessage(fullScreenChatBlockDataList[index].DisplayName, fullScreenChatBlockDataList[index].Message, fullScreenChatBlockDataList[index].IsChatActivity, fullScreenChatBlockDataList[index].IsAwatingModeration, fullScreenChatBlockDataList[index].IsChatBlocked);
			component.OnClicked = (Action<long>)Delegate.Combine(component.OnClicked, new Action<long>(onFullScreenChatBlockClicked));
			if (!imageRequests.Contains(fullScreenChatBlockDataList[index].SessionId))
			{
				DataEntityHandle handle = findPlayerHandle(fullScreenChatBlockDataList[index].SessionId);
				imageRequests.Enqueue(fullScreenChatBlockDataList[index].SessionId);
				AvatarAnimationFrame avatarAnimationFrame = new AvatarAnimationFrame("Base Layer.Idle", 0.5f);
				avatarImageComponent.RequestImage(handle, avatarAnimationFrame, "FullScreenChatAvatar");
			}
		}

		private void onElementHidden(int index, GameObject element)
		{
			FullScreenChatBlock component = element.GetComponent<FullScreenChatBlock>();
			component.EnablePreloader();
			component.OnClicked = (Action<long>)Delegate.Remove(component.OnClicked, new Action<long>(onFullScreenChatBlockClicked));
		}

		private void onFullScreenChatBlockClicked(long sessionId)
		{
			DataEntityHandle handle = findPlayerHandle(sessionId);
			OpenPlayerCardCommand openPlayerCardCommand = new OpenPlayerCardCommand(handle);
			openPlayerCardCommand.Execute();
		}

		private bool chatMessageReceived(ChatServiceEvents.ChatMessageReceived evt)
		{
			if (dataEntityCollection.LocalPlayerSessionId == evt.SessionId)
			{
				int chatAwaitingModerationBlockDataListIndex = getChatAwaitingModerationBlockDataListIndex(evt.SessionId);
				if (chatAwaitingModerationBlockDataListIndex != -1)
				{
					fullScreenChatBlockDataList.RemoveAt(chatAwaitingModerationBlockDataListIndex);
					layoutElementPool.RemoveElement(chatAwaitingModerationBlockDataListIndex);
				}
			}
			DataEntityHandle handle = findPlayerHandle(evt.SessionId);
			addChatMessage(evt.SessionId, evt.Message, handle);
			if (layoutElementPool.ElementScrollRect.verticalNormalizedPosition > 0f)
			{
				if (!JumpToNewMessageButton.gameObject.activeSelf)
				{
					JumpToNewMessageButton.gameObject.SetActive(true);
				}
			}
			else if (JumpToNewMessageButton.gameObject.activeSelf)
			{
				JumpToNewMessageButton.gameObject.SetActive(false);
			}
			EventManager.Instance.PostEvent("SFX/UI/Quest/Text/Open", EventAction.PlaySound);
			return false;
		}

		private bool onChatMessageBlockedReceived(ChatServiceEvents.ChatMessageBlockedReceived evt)
		{
			int chatAwaitingModerationBlockDataListIndex = getChatAwaitingModerationBlockDataListIndex(evt.SessionId);
			if (chatAwaitingModerationBlockDataListIndex != -1)
			{
				fullScreenChatBlockDataList.RemoveAt(chatAwaitingModerationBlockDataListIndex);
				layoutElementPool.RemoveElement(chatAwaitingModerationBlockDataListIndex);
			}
			addChatMessage(dataEntityCollection.LocalPlayerSessionId, "", dataEntityCollection.LocalPlayerHandle, false, false, true);
			return false;
		}

		private bool onSendChatMessage(ChatMessageSender.SendChatMessage evt)
		{
			addChatMessage(dataEntityCollection.LocalPlayerSessionId, evt.Message, dataEntityCollection.LocalPlayerHandle, false, true);
			jumpToNewMessage();
			return false;
		}

		private void addChatMessage(long sessionId, string message, DataEntityHandle handle, bool isChatActivity = false, bool isAwaitingModeration = false, bool isChatBlocked = false)
		{
			int chatActivityBlockDataListIndex = getChatActivityBlockDataListIndex(sessionId);
			if (chatActivityBlockDataListIndex != -1)
			{
				fullScreenChatBlockDataList.RemoveAt(chatActivityBlockDataListIndex);
				layoutElementPool.RemoveElement(chatActivityBlockDataListIndex);
			}
			addChatMessageToData(sessionId, message, handle, isChatActivity, isAwaitingModeration, isChatBlocked);
			bool flag = fullScreenChatBlockDataList.Count > 50;
			if (flag)
			{
				fullScreenChatBlockDataList.RemoveAt(0);
			}
			if (isPoolReady)
			{
				if (flag)
				{
					layoutElementPool.RemoveElement(0);
				}
				addChatMessageToPool(sessionId);
			}
		}

		private int getChatAwaitingModerationBlockDataListIndex(long sessionId)
		{
			for (int i = 0; i < fullScreenChatBlockDataList.Count; i++)
			{
				if (fullScreenChatBlockDataList[i].SessionId == sessionId && fullScreenChatBlockDataList[i].IsAwatingModeration)
				{
					return i;
				}
			}
			return -1;
		}

		private int getChatActivityBlockDataListIndex(long sessionId)
		{
			for (int i = 0; i < fullScreenChatBlockDataList.Count; i++)
			{
				if (fullScreenChatBlockDataList[i].SessionId == sessionId && fullScreenChatBlockDataList[i].IsChatActivity)
				{
					return i;
				}
			}
			return -1;
		}

		private void showChatActivity(long sessionId)
		{
			int chatActivityBlockDataListIndex = getChatActivityBlockDataListIndex(sessionId);
			if (chatActivityBlockDataListIndex == -1)
			{
				DataEntityHandle handle = findPlayerHandle(sessionId);
				addChatMessage(sessionId, "", handle, true);
			}
		}

		private void cancelChatActivity(long sessionId)
		{
			int chatActivityBlockDataListIndex = getChatActivityBlockDataListIndex(sessionId);
			if (chatActivityBlockDataListIndex != -1)
			{
				fullScreenChatBlockDataList.RemoveAt(chatActivityBlockDataListIndex);
				layoutElementPool.RemoveElement(chatActivityBlockDataListIndex);
			}
		}

		private bool onSendChatActivity(ChatServiceEvents.SendChatActivity evt)
		{
			showChatActivity(dataEntityCollection.LocalPlayerSessionId);
			layoutElementPool.ElementScrollRect.verticalNormalizedPosition = 0f;
			return false;
		}

		private bool onSendChatActivityCancel(ChatServiceEvents.SendChatActivityCancel evt)
		{
			cancelChatActivity(dataEntityCollection.LocalPlayerSessionId);
			return false;
		}

		private bool onChatActivityReceived(ChatServiceEvents.ChatActivityReceived evt)
		{
			showChatActivity(evt.SessionId);
			return false;
		}

		private bool onChatActivityCancelReceived(ChatServiceEvents.ChatActivityCancelReceived evt)
		{
			cancelChatActivity(evt.SessionId);
			return false;
		}

		private void onImageReady(DataEntityHandle handle, Texture2D icon)
		{
			long sessionId = dataEntityCollection.GetComponent<SessionIdData>(handle).SessionId;
			for (int i = 0; i < fullScreenChatBlockDataList.Count; i++)
			{
				if (fullScreenChatBlockDataList[i].SessionId == sessionId && layoutElementPool.IsElementVisible(i))
				{
					GameObject elementAtIndex = layoutElementPool.GetElementAtIndex(i);
					FullScreenChatBlock component = elementAtIndex.GetComponent<FullScreenChatBlock>();
					component.SetIcon(icon);
				}
			}
			imageRequests.Dequeue();
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			ClubPenguin.Core.SceneRefs.Get<CameraRenderingControl>().EnableRendering();
		}
	}
}
