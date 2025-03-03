using ClubPenguin.Core;
using DevonLocalization.Core;
using System;
using System.Collections.Generic;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class CellPhoneDailySpinActivityDefinition : CellPhoneActivityDefinition
	{
		[Serializable]
		public struct SpinReward
		{
			public int SpinOutcomeId;

			public int Weight;

			public RewardDefinition Reward;

			public int WheelPosition;
		}

		[Serializable]
		public struct ChestDefinition
		{
			public int ChestId;

			public int NumPunchesPerChest;

			public int NumChestsToNextLevel;

			public bool IsChestSpinNotAllowed;

			public RewardDefinition FirstTimeClaimedReward;

			[LocalizationToken]
			public string ChestTypeToken;

			[LocalizationToken]
			public string ChestNameToken;

			[LocalizationToken]
			public string ChestNameLevelToken;

			public List<ChestReward> RepeatableChestRewards;

			public List<ChestReward> NonRepeatableChestRewards;
		}

		[Serializable]
		public struct ChestReward
		{
			public int RewardId;

			public bool IsMemberOnly;

			public RewardDefinition Reward;
		}

		public List<SpinReward> SpinRewards;

		public List<ChestDefinition> ChestDefinitions;

		public RewardDefinition DefaultReward;

		public SpinReward FirstTimeSpinReward;

		public int InitialChestWeight = 5;

		public int ChestWeightIncreasePerSpin = 5;

		public int ChestSpinOutcomeId = 0;

		public int ChestWheelPosition;

		public int InitialRespinWeight = 5;

		public int RespinWeightIncreasePerSpin = 5;

		public RewardDefinition RespinReward;

		public int RespinSpinOutcomeId = 1;

		public int RespinWheelPosition;
	}
}
