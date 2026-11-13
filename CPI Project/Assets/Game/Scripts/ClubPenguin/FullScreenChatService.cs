using ClubPenguin.Chat;
using ClubPenguin.Net;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class FullScreenChatService : AbstractDataModelService
	{
		private List<FullScreenChatData> fullScreenChatDataList;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<ChatServiceEvents.ChatMessageReceived>(onChatMessageReceived);
			Service.Get<EventDispatcher>().AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			fullScreenChatDataList = new List<FullScreenChatData>();
		}

		private bool onChatMessageReceived(ChatServiceEvents.ChatMessageReceived evt)
		{
			ChatHistoryData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				component.AddMessage(new DChatMessage(evt.SessionId, evt.Message, evt.SizzleClipID));
			}
			FullScreenChatData fullScreenChatData;
			if (dataEntityCollection.IsLocalPlayer(evt.SessionId))
			{
				fullScreenChatDataList.Add(null);
				checkListLength();
			}
			else if (tryGetFullScreenChatData(evt.SessionId, out fullScreenChatData))
			{
				fullScreenChatData.MessageCount++;
				fullScreenChatDataList.Add(fullScreenChatData);
				checkListLength();
			}
			else if (!Service.Get<ZoneTransitionService>().IsTransitioning)
			{
				Log.LogErrorFormatted(this, "Could not find a player with the session id {0}", evt.SessionId);
			}
			return false;
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Begin)
			{
				fullScreenChatDataList.Clear();
			}
			return false;
		}

		private void checkListLength()
		{
			while (fullScreenChatDataList.Count > 50)
			{
				if (fullScreenChatDataList[0] != null)
				{
					fullScreenChatDataList[0].MessageCount--;
					if (fullScreenChatDataList[0].MessageCount <= 0)
					{
						DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(fullScreenChatDataList[0]);
						dataEntityCollection.RemoveComponent<FullScreenChatData>(entityByComponent);
					}
				}
				fullScreenChatDataList.RemoveAt(0);
			}
		}

		private bool tryGetFullScreenChatData(long sessionId, out FullScreenChatData fullScreenChatData)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(sessionId);
			if (!dataEntityHandle.IsNull)
			{
				if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out fullScreenChatData))
				{
					fullScreenChatData = dataEntityCollection.AddComponent<FullScreenChatData>(dataEntityHandle);
				}
				return true;
			}
			fullScreenChatData = null;
			return false;
		}
	}
}
