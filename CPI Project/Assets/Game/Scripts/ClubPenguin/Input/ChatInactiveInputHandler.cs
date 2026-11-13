using UnityEngine;

namespace ClubPenguin.Input
{
	public class ChatInactiveInputHandler : InputMapHandler<ChatInactiveInputMap.Result>
	{
		[SerializeField]
		private InputMappedButton btnChat = null;

		[SerializeField]
		private InputMappedButton btnQuickEmoji = null;

		[SerializeField]
		private InputMappedButton btnQuickChat = null;

		protected override void onHandle(ChatInactiveInputMap.Result inputResult)
		{
			btnChat.HandleMappedInput(inputResult.Chat);
			btnQuickEmoji.HandleMappedInput(inputResult.QuickEmote);
			btnQuickChat.HandleMappedInput(inputResult.QuickChat);
		}

		protected override void onReset()
		{
			btnChat.HandleMappedInput();
			btnQuickEmoji.HandleMappedInput();
			btnQuickChat.HandleMappedInput();
		}
	}
}
