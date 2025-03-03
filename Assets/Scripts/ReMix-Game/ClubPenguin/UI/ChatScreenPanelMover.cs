using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ChatScreenPanelMover : MonoBehaviour
	{
		public float InstantPosition;

		public float InputPosition;

		private RectTransform rectTransform;

		private Transform root;

		private ChatBarController chatBarController;

		private void Start()
		{
			rectTransform = (base.transform as RectTransform);
			root = GetComponentInParent<Canvas>().transform;
			chatBarController = root.GetComponentInChildren<ChatBarController>();
			if (chatBarController != null)
			{
				updateState(chatBarController.CurrentState);
			}
			Service.Get<EventDispatcher>().AddListener<ChatBarEvents.ChatBarStateChanged>(onChatBarStateChanged, EventDispatcher.Priority.HIGH);
		}

		private void updateState(ChatBarState chatBarState)
		{
			if (chatBarController == null)
			{
				chatBarController = root.GetComponentInChildren<ChatBarController>();
			}
			if (chatBarState == ChatBarState.EmoteInput)
			{
				rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, InputPosition);
			}
			else
			{
				rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, InstantPosition);
			}
		}

		private bool onChatBarStateChanged(ChatBarEvents.ChatBarStateChanged evt)
		{
			updateState(evt.ChatBarState);
			return false;
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<ChatBarEvents.ChatBarStateChanged>(onChatBarStateChanged);
		}
	}
}
