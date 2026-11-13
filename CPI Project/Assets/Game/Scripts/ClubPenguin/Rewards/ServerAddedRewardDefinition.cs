using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/ServerAddedReward")]
	public class ServerAddedRewardDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public int Id;

		[LocalizationToken]
		public string TitleToken;

		public bool IsMemberOnly;

		public bool ClaimOnLogin = true;

		public RewardDefinition Reward;
	}
}
