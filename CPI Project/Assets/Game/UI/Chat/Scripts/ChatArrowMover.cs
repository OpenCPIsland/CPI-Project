using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Chat
{
	public class ChatArrowMover : MonoBehaviour
	{
		private Image chatArrowImage;

		private Transform root;

		private ChatBarController chatBarController;

		private void Start()
		{
			chatArrowImage = GetComponentInChildren<Image>();
			root = GetComponentInParent<StateMachineContext>().transform;
			chatBarController = root.GetComponentInChildren<ChatBarController>();
			if (chatBarController != null)
			{
				updateState(chatBarController.CurrentState);
			}
			else
			{
				chatArrowImage.enabled = false;
			}
			Service.Get<EventDispatcher>().AddListener<ChatBarEvents.ChatBarStateChanged>(onChatBarStateChanged, EventDispatcher.Priority.LOW);
		}

		private void updateState(ChatBarState chatBarState)
		{
			if (chatBarController == null)
			{
				chatBarController = root.GetComponentInChildren<ChatBarController>();
			}
			switch (chatBarState)
			{
			case ChatBarState.Instant:
				base.transform.position = chatBarController.QuickChatLocation.position;
				chatArrowImage.enabled = true;
				break;
			case ChatBarState.EmoteInstant:
				base.transform.position = chatBarController.QuickEmoteLocation.position;
				chatArrowImage.enabled = true;
				break;
			case ChatBarState.EmoteInput:
				base.transform.position = chatBarController.EmoteLocation.position;
				chatArrowImage.enabled = true;
				break;
			case ChatBarState.Default:
				chatArrowImage.enabled = false;
				break;
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
