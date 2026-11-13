using System.Runtime.InteropServices;

namespace ClubPenguin.IslandTargets
{
	public class IslandTargetsEvents
	{
		public struct TargetsRemainingUpdated
		{
			public int TargetsRemaining;

			public int TotalTargets;

			public TargetsRemainingUpdated(int targetsRemaining, int totalTargets)
			{
				TargetsRemaining = targetsRemaining;
				TotalTargets = totalTargets;
			}
		}

		public struct GameRoundStarted
		{
			public long StartTime;

			public long EndTime;

			public bool IsFinalRound;

			public GameRoundStarted(long startTime, long endTime, bool isFinalRound)
			{
				StartTime = startTime;
				EndTime = endTime;
				IsFinalRound = isFinalRound;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct GameRoundEnded
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct TargetGameTimeOut
		{
		}

		public struct TargetHit
		{
			public float DamageCapacity;

			public float DamageCount;

			public TargetHit(float damageCapacity, float damageCount)
			{
				DamageCapacity = damageCapacity;
				DamageCount = damageCount;
			}
		}

		public struct StatsUpdated
		{
			public int BestWinStreak;

			public int CurrentWinStreak;

			public StatsUpdated(int bestWinStreak, int currentWinStreak)
			{
				BestWinStreak = bestWinStreak;
				CurrentWinStreak = currentWinStreak;
			}
		}

		public struct LocalPlayerHitTargetEvent
		{
			public readonly IslandTarget Target;

			public LocalPlayerHitTargetEvent(IslandTarget target)
			{
				Target = target;
			}
		}

		public struct ClockTowerStateChanged
		{
			public bool IsEnabled;

			public ClockTowerStateChanged(bool isEnabled)
			{
				IsEnabled = isEnabled;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct LocalPlayerParticipated
		{
		}
	}
}
