using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class ToggleChatMode : MonoBehaviour
	{
		public Camera GameCamera;

		public ChatMode ActiveChatMode = ChatMode.WORLD;

		private WorldChatController worldChat;

		private void Awake()
		{
			worldChat = GetComponentInChildren<WorldChatController>();
		}

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<ChatEvents.ShowFullScreen>(onShowFullScreen);
			Service.Get<EventDispatcher>().AddListener<ChatEvents.HideFullScreen>(onHideFullScreen);
			CoroutineRunner.Start(initializeChatMode(), this, "initializeChatMode");
		}

		private IEnumerator initializeChatMode()
		{
			yield return new WaitForEndOfFrame();
			if (ActiveChatMode == ChatMode.FULL_SCREEN)
			{
				worldChat.gameObject.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<ChatEvents.ShowFullScreen>(onShowFullScreen);
			Service.Get<EventDispatcher>().RemoveListener<ChatEvents.HideFullScreen>(onHideFullScreen);
		}

		private bool onShowFullScreen(ChatEvents.ShowFullScreen evt)
		{
			worldChat.ClearSpeechBubbles();
			worldChat.gameObject.SetActive(false);
			ActiveChatMode = ChatMode.FULL_SCREEN;
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.HidePlayerNames));
			return false;
		}

		private bool onHideFullScreen(ChatEvents.HideFullScreen evt)
		{
			worldChat.gameObject.SetActive(true);
			ActiveChatMode = ChatMode.WORLD;
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.ShowPlayerNames));
			return false;
		}
	}
}
