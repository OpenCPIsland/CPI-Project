using ClubPenguin.Chat;
using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using System.Collections.Generic;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class EmoteMatcherAction : FsmStateAction
	{
		public string[] EmoteNames;

		public FsmEvent MatchEvent;

		public FsmEvent FailedEvent;

		public bool IsIncludingRemotePlayers;

		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<ChatEvents.ChatEmoteMessageShown>(OnChatEmoteMessage);
		}

		private bool OnChatEmoteMessage(ChatEvents.ChatEmoteMessageShown evt)
		{
			if (!IsIncludingRemotePlayers && !isSessionIdLocalPlayer(evt.SessionId))
			{
				return false;
			}
			Dictionary<string, EmoteDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, EmoteDefinition>>();
			EmoteDefinition value = null;
			int num = 0;
			for (int i = 0; i < evt.Message.Length; i++)
			{
				if (EmoteManager.IsEmoteCharacter(evt.Message[i]))
				{
					dictionary.TryGetValue(EmoteNames[num], out value);
					if (!(value != null))
					{
						continue;
					}
					if (evt.Message[i] == EmoteManager.GetEmoteChar(value))
					{
						num++;
						if (num >= EmoteNames.Length)
						{
							base.Fsm.Event(MatchEvent);
							break;
						}
						continue;
					}
					if (EmoteNames.Length == 1 && i == evt.Message.Length - 1)
					{
						base.Fsm.Event(FailedEvent);
						break;
					}
					if (num != 0)
					{
						num = 0;
						i--;
					}
				}
				else if (num != 0)
				{
					break;
				}
			}
			return false;
		}

		private bool isSessionIdLocalPlayer(long sessionId)
		{
			return sessionId == Service.Get<CPDataEntityCollection>().LocalPlayerSessionId;
		}

		public override void OnExit()
		{
			dispatcher.RemoveListener<ChatEvents.ChatEmoteMessageShown>(OnChatEmoteMessage);
		}
	}
}
