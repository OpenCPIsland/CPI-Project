using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.Net
{
	public class PartyGameService : BaseNetworkService, IPartyGameService, INetworkService
	{
		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.PARTY_GAME_START, onPartyGameStart);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.PARTY_GAME_START_V2, onPartyGameStartV2);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.PARTY_GAME_END, onPartyGameEnd);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.PARTY_GAME_MESSAGE, onPartyGameMessage);
		}

		public void SendSessionMessage(int sessionId, int type, object data)
		{
			clubPenguinClient.GameServer.SendPartyGameSessionMessage(sessionId, type, data);
		}

		private void onPartyGameStart(GameServerEvent gameServerEvent, object data)
		{
			PartyGameStartEvent partyGameStartEvent = (PartyGameStartEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PartyGameServiceEvents.PartyGameStarted(partyGameStartEvent.owner, partyGameStartEvent.players, partyGameStartEvent.sessionId, partyGameStartEvent.templateId));
		}

		private void onPartyGameStartV2(GameServerEvent gameServerEvent, object data)
		{
			PartyGameStartEventV2 partyGameStartEventV = (PartyGameStartEventV2)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PartyGameServiceEvents.PartyGameStartedV2(partyGameStartEventV.sessionId, partyGameStartEventV.templateId, partyGameStartEventV.playerData));
		}

		private void onPartyGameEnd(GameServerEvent gameServerEvent, object data)
		{
			PartyGameEndEvent partyGameEndEvent = (PartyGameEndEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PartyGameServiceEvents.PartyGameEnded(partyGameEndEvent.playerSessionIdToPlacement, partyGameEndEvent.sessionId));
		}

		private void onPartyGameMessage(GameServerEvent gameServerEvent, object data)
		{
			PartyGameMessageEvent partyGameMessageEvent = (PartyGameMessageEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PartyGameServiceEvents.PartyGameSessionMessage(partyGameMessageEvent.sessionId, partyGameMessageEvent.type, partyGameMessageEvent.message));
		}
	}
}
