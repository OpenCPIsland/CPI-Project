using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.PartyGames
{
	public abstract class AbstractPartyGameSession : IPartyGameSession
	{
		private const float AUDIO_PREFAB_DESTROY_DELAY = 3f;

		protected GameObject audioPrefab;

		protected int partyGameId;

		private int id;

		private List<PartyGamePlayer> playersList;

		protected int sessionId
		{
			get
			{
				return id;
			}
		}

		protected List<PartyGamePlayer> players
		{
			get
			{
				return playersList;
			}
		}

		public void StartGame(int id, List<PartyGamePlayer> players, int partyGameId)
		{
			this.id = id;
			playersList = players;
			this.partyGameId = partyGameId;
			startGame();
		}

		protected void loadAudioPrefab(PartyGameDefinition definition)
		{
			if (!string.IsNullOrEmpty(definition.AudioPrefab.Key))
			{
				Content.LoadAsync(onAudioPrefabLoaded, definition.AudioPrefab);
			}
		}

		private void onAudioPrefabLoaded(string path, GameObject prefab)
		{
			audioPrefab = Object.Instantiate(prefab);
			audioPrefabLoaded();
		}

		protected virtual void audioPrefabLoaded()
		{
		}

		public void EndGame(Dictionary<long, int> playerSessionIdToPlacement)
		{
			endGame(playerSessionIdToPlacement);
			if (audioPrefab != null)
			{
				Object.Destroy(audioPrefab, 3f);
			}
			destroy();
		}

		public void HandleSessionMessage(PartyGameSessionMessageTypes type, string data)
		{
			handleSessionMessage(type, data);
		}

		protected abstract void handleSessionMessage(PartyGameSessionMessageTypes type, string data);

		protected abstract void startGame();

		protected abstract void endGame(Dictionary<long, int> playerSessionIdToPlacement);

		protected abstract void destroy();

		protected void sendSessionMessage(PartyGameSessionMessageTypes type, object data)
		{
			Service.Get<INetworkServicesManager>().PartyGameService.SendSessionMessage(id, (int)type, data);
		}

		protected PartyGameDefinition getPartyGameDefinition(int definitionId)
		{
			Dictionary<int, PartyGameDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, PartyGameDefinition>>();
			if (!dictionary.ContainsKey(definitionId))
			{
			}
			return dictionary[definitionId];
		}
	}
}
