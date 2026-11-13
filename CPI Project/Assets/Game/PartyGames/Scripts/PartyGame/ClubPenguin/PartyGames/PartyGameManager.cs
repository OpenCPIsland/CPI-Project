using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin.PartyGames
{
	public class PartyGameManager
	{
		private Dictionary<int, IPartyGameSession> sessions;

		private readonly IPartyGameSessionFactory partyGameSessionFactory;

		public PartyGameManager(IPartyGameSessionFactory partyGameSessionFactory)
		{
			this.partyGameSessionFactory = partyGameSessionFactory;
			sessions = new Dictionary<int, IPartyGameSession>();
			addEventListeners();
		}

		public void Reset()
		{
			destroyAllSessions();
		}

		private void addEventListeners()
		{
			Service.Get<EventDispatcher>().AddListener<PartyGameServiceEvents.PartyGameStarted>(onPartyGameStarted);
			Service.Get<EventDispatcher>().AddListener<PartyGameServiceEvents.PartyGameStartedV2>(onPartyGameStartedV2);
			Service.Get<EventDispatcher>().AddListener<PartyGameServiceEvents.PartyGameEnded>(onPartyGameEnded);
			Service.Get<EventDispatcher>().AddListener<PartyGameServiceEvents.PartyGameSessionMessage>(onPartyGameMessage);
			Service.Get<EventDispatcher>().AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransitionStart);
		}

		private bool onPartyGameStarted(PartyGameServiceEvents.PartyGameStarted evt)
		{
			List<PartyGamePlayer> list = new List<PartyGamePlayer>();
			list.Add(new PartyGamePlayer(0, evt.Owner, 0));
			for (int i = 0; i < evt.Players.Length; i++)
			{
				list.Add(new PartyGamePlayer(0, evt.Players[i], 0));
			}
			startSession(evt.SessionId, evt.GameTemplateId, list);
			return false;
		}

		private bool onPartyGameStartedV2(PartyGameServiceEvents.PartyGameStartedV2 evt)
		{
			PartyGamePlayerCollection partyGamePlayerCollection = Service.Get<JsonService>().Deserialize<PartyGamePlayerCollection>(evt.PlayerData);
			startSession(evt.SessionId, evt.GameTemplateId, partyGamePlayerCollection.Players);
			return false;
		}

		private bool onPartyGameEnded(PartyGameServiceEvents.PartyGameEnded evt)
		{
			endSession(evt.SessionId, evt.PlayerSessionIdToPlacement);
			return false;
		}

		private bool onPartyGameMessage(PartyGameServiceEvents.PartyGameSessionMessage evt)
		{
			handleSessionMessage(evt.SessionId, (PartyGameSessionMessageTypes)evt.Type, evt.Data);
			return false;
		}

		private bool onSceneTransitionStart(SceneTransitionEvents.TransitionStart evt)
		{
			destroyAllSessions();
			return false;
		}

		private void startSession(int sessionId, int partyGameId, List<PartyGamePlayer> players)
		{
			IPartyGameSession sessionTemplate = getSessionTemplate(partyGameId);
			if (sessions.ContainsKey(sessionId))
			{
				sessions[sessionId].EndGame(new Dictionary<long, int>());
				sessions.Remove(sessionId);
			}
			sessions.Add(sessionId, sessionTemplate);
			sessionTemplate.StartGame(sessionId, players, partyGameId);
		}

		private void endSession(int sessionId, Dictionary<long, int> playerSessionIdToPlacement)
		{
			IPartyGameSession value;
			if (sessions.TryGetValue(sessionId, out value))
			{
				value.EndGame(playerSessionIdToPlacement);
				sessions.Remove(sessionId);
			}
		}

		private void handleSessionMessage(int sessionId, PartyGameSessionMessageTypes type, string jsonData)
		{
			IPartyGameSession value;
			if (sessions.TryGetValue(sessionId, out value))
			{
				value.HandleSessionMessage(type, jsonData);
			}
		}

		private void destroyAllSessions()
		{
			foreach (IPartyGameSession value in sessions.Values)
			{
				value.EndGame(new Dictionary<long, int>());
			}
			sessions.Clear();
		}

		private IPartyGameSession getSessionTemplate(int templateId)
		{
			return partyGameSessionFactory.getPartyGameSession((PartyGameDefinition.GameTypes)templateId);
		}
	}
}
