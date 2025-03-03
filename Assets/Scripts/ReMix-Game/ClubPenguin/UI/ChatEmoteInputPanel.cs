using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ChatEmoteInputPanel : MonoBehaviour
	{
		public void OnBackSpaceClicked()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(ChatEvents.ChatBackSpace));
		}
	}
}
