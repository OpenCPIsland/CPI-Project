using ClubPenguin.Net.Domain;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class RewardServiceEvents
	{
		public struct RewardsEarned
		{
			public readonly RewardedUserCollection RewardedUsers;

			public RewardsEarned(RewardedUserCollection rewardedUsers)
			{
				RewardedUsers = rewardedUsers;
			}
		}

		public struct MyAssetsReceived
		{
			public readonly PlayerAssets Assets;

			public MyAssetsReceived(PlayerAssets assets)
			{
				Assets = assets;
			}
		}

		public struct MyRewardEarned
		{
			public readonly RewardSource Source;

			public readonly string SourceId;

			public readonly Reward Reward;

			public readonly bool ShowReward;

			public MyRewardEarned(RewardSource source, string sourceId, Reward reward, bool showReward = true)
			{
				Source = source;
				SourceId = sourceId;
				Reward = reward;
				ShowReward = showReward;
			}
		}

		public struct ClaimedReward
		{
			public readonly Reward Reward;

			public ClaimedReward(Reward reward)
			{
				Reward = reward;
			}
		}

		public struct ClaimableRewardFail
		{
			public readonly int RewardId;

			public ClaimableRewardFail(int rewardId)
			{
				RewardId = rewardId;
			}
		}

		public struct ClaimQuickNotificationRewardSuccess
		{
			public readonly Reward Reward;

			public ClaimQuickNotificationRewardSuccess(Reward reward)
			{
				Reward = reward;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ClaimQuickNotificationRewardFailed
		{
		}

		public struct ClaimDailySpinRewardSuccess
		{
			public readonly Reward Reward;

			public readonly Reward ChestReward;

			public readonly int SpinOutcomeId;

			public ClaimDailySpinRewardSuccess(Reward reward, Reward chestReward, int spinOutcomeId)
			{
				Reward = reward;
				ChestReward = chestReward;
				SpinOutcomeId = spinOutcomeId;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ClaimDailySpinRewardFailed
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ClaimPreregistrationRewardNotFound
		{
		}

		public struct ClaimPreregistrationRewardFound
		{
			public readonly Reward Reward;

			public ClaimPreregistrationRewardFound(Reward reward)
			{
				Reward = reward;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ClaimServerAddedRewardsNotFound
		{
		}

		public struct ClaimServerAddedRewardsFound
		{
			public readonly List<ServerAddedReward> ServerAddedRewards;

			public ClaimServerAddedRewardsFound(List<ServerAddedReward> serverAddedRewards)
			{
				ServerAddedRewards = serverAddedRewards;
			}
		}

		public struct MyRewardCalculated
		{
			public readonly int Coins;

			public MyRewardCalculated(int coin)
			{
				Coins = coin;
			}
		}

		public struct RoomRewardsReceived
		{
			public readonly string Room;

			public readonly Dictionary<string, long> EarnedRewards;

			public RoomRewardsReceived(string room, Dictionary<string, long> earnedRewards)
			{
				EarnedRewards = earnedRewards;
				Room = room;
			}
		}

		public struct LevelUp
		{
			public readonly int Level;

			public readonly long SessionId;

			public LevelUp(long sessionId, int level)
			{
				SessionId = sessionId;
				Level = level;
			}
		}

		public struct ClaimDelayedReward
		{
			public RewardSource Source;

			public string SourceId;

			public ClaimDelayedReward(RewardSource source, string sourceId)
			{
				Source = source;
				SourceId = sourceId;
			}
		}
	}
}
