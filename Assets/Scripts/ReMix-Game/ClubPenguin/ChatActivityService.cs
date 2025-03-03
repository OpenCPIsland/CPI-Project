using ClubPenguin.Net;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;

namespace ClubPenguin
{
	public class ChatActivityService : AbstractDataModelService
	{
		private ChatActivityData chatActivityData;

		private void Start()
		{
			DataEntityHandle handle = dataEntityCollection.AddEntity("ChatActivity");
			chatActivityData = dataEntityCollection.AddComponent<ChatActivityData>(handle);
			ChatActivityData obj = chatActivityData;
			obj.OnTimeOutComplete = (System.Action)Delegate.Combine(obj.OnTimeOutComplete, new System.Action(onTimeOutComplete));
			ChatActivityData obj2 = chatActivityData;
			obj2.SendChatActivity = (System.Action)Delegate.Combine(obj2.SendChatActivity, new System.Action(onSendChatActivity));
			Service.Get<EventDispatcher>().AddListener<ChatActivityServiceEvents.SendChatActivity>(onSendChatActivity);
			Service.Get<EventDispatcher>().AddListener<ChatActivityServiceEvents.SendChatActivityCancel>(onSendChatActivityCancel);
			Service.Get<EventDispatcher>().AddListener<ChatMessageSender.SendChatMessage>(onSendChatMessage);
		}

		private bool onSendChatActivity(ChatActivityServiceEvents.SendChatActivity evt)
		{
			if (!chatActivityData.IsChatActive)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(ChatServiceEvents.SendChatActivity));
			}
			chatActivityData.OnSendChatActivity();
			return true;
		}

		private bool onSendChatActivityCancel(ChatActivityServiceEvents.SendChatActivityCancel evt)
		{
			if (chatActivityData.IsChatActive)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(ChatServiceEvents.SendChatActivityCancel));
			}
			chatActivityData.OnSetChatActiveCancel();
			return true;
		}

		private bool onSendChatMessage(ChatMessageSender.SendChatMessage evt)
		{
			if (chatActivityData.IsChatActive)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(ChatServiceEvents.SendChatActivityCancel));
			}
			chatActivityData.OnSendChatMessage();
			return false;
		}

		private void onTimeOutComplete()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(ChatServiceEvents.SendChatActivityCancel));
		}

		private void onSendChatActivity()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(ChatServiceEvents.SendChatActivity));
		}
	}
}
