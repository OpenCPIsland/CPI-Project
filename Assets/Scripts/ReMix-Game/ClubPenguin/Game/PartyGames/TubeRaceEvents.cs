using ClubPenguin.PartyGames;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ClubPenguin.Game.PartyGames
{
	public class TubeRaceEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct LocalPlayerJoinedLobby
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct LocalPlayerLeftLobby
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CloseLobby
		{
		}

		public struct RaceStart
		{
			public readonly PartyGameDefinition.GameTypes RaceType;

			public RaceStart(PartyGameDefinition.GameTypes raceType)
			{
				RaceType = raceType;
			}
		}

		public struct RaceEnd
		{
			public readonly PartyGameDefinition.GameTypes RaceType;

			public RaceEnd(PartyGameDefinition.GameTypes raceType)
			{
				RaceType = raceType;
			}
		}

		public struct ScoreModifierCollected
		{
			public readonly int ScoreModifierId;

			public readonly int ScoreDelta;

			public ScoreModifierCollected(int scoreModifierId, int scoreDelta)
			{
				ScoreModifierId = scoreModifierId;
				ScoreDelta = scoreDelta;
			}
		}

		public struct SetModifierLayout
		{
			public readonly int ScoreModifierLayoutId;

			public readonly PartyGameDefinition.GameTypes RaceType;

			public SetModifierLayout(PartyGameDefinition.GameTypes raceType, int scoreModifierLayoutId)
			{
				RaceType = raceType;
				ScoreModifierLayoutId = scoreModifierLayoutId;
			}
		}

		public struct PlayerResultReceived
		{
			public readonly PartyGameSessionMessages.TubeRacePlayerResult PlayerResult;

			public PlayerResultReceived(PartyGameSessionMessages.TubeRacePlayerResult playerResult)
			{
				PlayerResult = playerResult;
			}
		}

		public struct EndGameResultsReceived
		{
			public readonly Dictionary<long, int> PlayerIdToPlacement;

			public List<PartyGameSessionMessages.TubeRacePlayerResult> PlayerResults;

			public int PartyGameId;

			public EndGameResultsReceived(int partyGameId, Dictionary<long, int> playerIdToPlacement, List<PartyGameSessionMessages.TubeRacePlayerResult> playerResults)
			{
				PartyGameId = partyGameId;
				PlayerIdToPlacement = playerIdToPlacement;
				PlayerResults = playerResults;
			}
		}
	}
}
