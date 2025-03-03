using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.UI
{
	public class ChatScreenEmoteInstantStateHandler : AbstractStateHandler
	{
		protected override void OnEnter()
		{
			Service.Get<EventDispatcher>().AddListener<ChatEvents.EmoteSelected>(onEmoteSelected);
		}

		protected override void OnExit()
		{
			Service.Get<EventDispatcher>().RemoveListener<ChatEvents.EmoteSelected>(onEmoteSelected);
		}

		private bool onEmoteSelected(ChatEvents.EmoteSelected evt)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new ChatMessageSender.SendChatMessage(evt.EmoteString, null));
			return false;
		}
	}
}
