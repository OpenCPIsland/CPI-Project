using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class ChatDisplayToggle : MonoBehaviour
	{
		private bool chatOpen;

		public bool ChatOpen
		{
			get
			{
				return chatOpen;
			}
			private set
			{
				if (value != chatOpen)
				{
					chatOpen = value;
					this.OnChatOpened.InvokeSafe(chatOpen);
				}
			}
		}

		public event Action<bool> OnChatOpened;

		private void OnEnable()
		{
			Service.Get<EventDispatcher>().AddListener<ChatEvents.ChatOpened>(onChatOpened);
			Service.Get<EventDispatcher>().AddListener<ChatEvents.ChatClosed>(onChatClosed);
		}

		private void OnDisable()
		{
			Service.Get<EventDispatcher>().RemoveListener<ChatEvents.ChatOpened>(onChatOpened);
			Service.Get<EventDispatcher>().RemoveListener<ChatEvents.ChatClosed>(onChatClosed);
		}

		private bool onChatOpened(ChatEvents.ChatOpened evt)
		{
			ChatOpen = true;
			return false;
		}

		private bool onChatClosed(ChatEvents.ChatClosed evt)
		{
			ChatOpen = false;
			return false;
		}
	}
}
