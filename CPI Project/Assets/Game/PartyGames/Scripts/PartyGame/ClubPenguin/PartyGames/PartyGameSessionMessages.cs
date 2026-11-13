using UnityEngine;

namespace ClubPenguin.PartyGames
{
	public class PartyGameSessionMessages
	{
		public class AddItem
		{
			public int ItemId;

			public Vector3 Pos;
		}

		public class RemoveItem
		{
			public int ItemId;

			public RemoveItem(int itemId)
			{
				ItemId = itemId;
			}

			public RemoveItem()
			{
			}
		}

		public class SetRole
		{
			public int RoleId;
		}

		public class SetGameState
		{
			public int GameStateId;
		}

		public class SetGameStartData
		{
			public Vector3 OwnerPosition;

			public Vector3[] PlayerPositions;

			public Vector3[] GamePositions;

			public int[] IntData;

			public SetGameStartData(Vector3 ownerPosition, Vector3[] playerPositions, Vector3[] gamePositions, int[] intData)
			{
				OwnerPosition = ownerPosition;
				PlayerPositions = playerPositions;
				GamePositions = gamePositions;
				IntData = intData;
			}

			public SetGameStartData()
			{
			}
		}

		public class ShowTurnOutput
		{
			public long PlayerId;

			public int ScoreDelta;

			public ShowTurnOutput(long playerId, int scoreDelta)
			{
				PlayerId = playerId;
				ScoreDelta = scoreDelta;
			}

			public ShowTurnOutput()
			{
			}
		}

		public class PlayerTurnStart
		{
			public long PlayerId;

			public PlayerTurnStart(int playerId)
			{
				PlayerId = playerId;
			}

			public PlayerTurnStart()
			{
			}
		}

		public class PlayerLeftGame
		{
			public long PlayerId;

			public PlayerLeftGame(long playerId)
			{
				PlayerId = playerId;
			}

			public PlayerLeftGame()
			{
			}
		}

		public class PlayDanceMoves
		{
			public int[] DanceMoveIds;
		}

		public class DanceMoveSequenceComplete
		{
		}

		public class SetTubeRaceModifierLayout
		{
			public int ModifierLayoutId;
		}

		public class CollectTubeRaceScoreModifier
		{
			public int ModifierId;

			public CollectTubeRaceScoreModifier(int modifierId)
			{
				ModifierId = modifierId;
			}
		}

		public class PlayerFinishedTubeRace
		{
		}

		public class TubeRacePlayerResult
		{
			public long PlayerId;

			public long CompletionTimeInMilliseconds;

			public float ScoreModifier;

			public float OverallScore;
		}
	}
}
