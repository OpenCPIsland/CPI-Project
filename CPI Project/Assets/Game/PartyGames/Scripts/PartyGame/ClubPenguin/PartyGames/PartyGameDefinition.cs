using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ClubPenguin.PartyGames
{
	[Serializable]
	public class PartyGameDefinition : StaticGameDataDefinition
	{
		public enum LobbyTypes
		{
			STRICT,
			STRICT_USER_START,
			MMOITEM_TEAM
		}

		public enum GameTypes
		{
			SCAVENGER_HUNT,
			FISH_BUCKET,
			FIND_FOUR,
			DANCE_BATTLE,
			TUBE_RACE_RED,
			TUBE_RACE_BLUE
		}

		[Serializable]
		public class PartyGameReward
		{
			public PartyGameEndPlacement Placement;

			public RewardDefinition Reward;
		}

		[StaticGameDataDefinitionId]
		public int Id;

		[LocalizationToken]
		[JsonIgnore]
		public string Name;

		public List<PartyGameReward> Rewards;

		public PartyGameDataDefinition GameData;

		public PartyGameLobbyDefinition LobbyData;

		public LobbyTypes LobbyType;

		public GameTypes GameType;

		public int MinPlayerCount;

		public int MaxPlayerCount;

		public PrefabContentKey AudioPrefab;

		public int NumTeams;

		public bool IsBlockingJumpToFriend;
	}
}
